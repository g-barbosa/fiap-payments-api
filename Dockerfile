# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/FiapCloudGames.Payments.API/FiapCloudGames.Payments.API.sln ./FiapCloudGames.Payments.API/
COPY src/FiapCloudGames.Payments.API/FiapCloudGames.Payments.API.csproj ./FiapCloudGames.Payments.API/
COPY src/FiapCloudGames.Payments.Application/FiapCloudGames.Payments.Application.csproj ./FiapCloudGames.Payments.Application/
COPY src/FiapCloudGames.Payments.Infrastructure/FiapCloudGames.Payments.Infrastructure.csproj ./FiapCloudGames.Payments.Infrastructure/
RUN dotnet restore ./FiapCloudGames.Payments.API/FiapCloudGames.Payments.API.sln

COPY src/FiapCloudGames.Payments.API/ ./FiapCloudGames.Payments.API/
COPY src/FiapCloudGames.Payments.Application/ ./FiapCloudGames.Payments.Application/
COPY src/FiapCloudGames.Payments.Infrastructure/ ./FiapCloudGames.Payments.Infrastructure/

WORKDIR /src/FiapCloudGames.Payments.API
RUN dotnet build FiapCloudGames.Payments.API.csproj -c Release

# Stage 2: Publicação
FROM build AS publish
RUN dotnet publish FiapCloudGames.Payments.API.csproj -c Release -o /app/publish

# Stage 3: Runtime (Final)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app

RUN addgroup -g 1000 -S appgroup && \
    adduser -u 1000 -S appuser -G appgroup


RUN apk add --no-cache \
    icu-libs \
    tzdata \
    ca-certificates


ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ=America/Sao_Paulo


RUN mkdir -p /app/logs && chown -R appuser:appgroup /app/logs

COPY --from=publish --chown=appuser:appgroup /app/publish .

USER appuser

EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FiapCloudGames.Payments.API.dll"]