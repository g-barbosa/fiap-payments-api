# FIAP Cloud Games - Payments API

API de Pagamentos do projeto FIAP Cloud Games, responsável por processar pagamentos de pedidos de jogos.

## 📋 Descrição

Este microserviço faz parte do ecossistema FIAP Cloud Games e é responsável por:

- Consumir eventos de pedidos criados via RabbitMQ
- Processar pagamentos de forma assíncrona
- Publicar eventos de pagamentos processados para outros serviços

## 🏗️ Arquitetura

O projeto segue a arquitetura Clean Architecture com as seguintes camadas:

```
src/
├── FiapCloudGames.Payments.API/            # Camada de apresentação (Controllers, Startup)
├── FiapCloudGames.Payments.Application/    # Casos de uso e serviços de aplicação
├── FiapCloudGames.Payments.Domain/         # Entidades e interfaces do domínio
└── FiapCloudGames.Payments.Infrastructure/ # Implementações (RabbitMQ, etc.)
```

## 🛠️ Tecnologias

- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **RabbitMQ** - Mensageria assíncrona
- **Docker** - Containerização
- **Kubernetes** - Orquestração de containers
- **Swagger/OpenAPI** - Documentação da API

## ⚙️ Variáveis de Ambiente

### Configurações da Aplicação

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execução (Development, Production) | `Production` |
| `ASPNETCORE_URLS` | URLs de escuta da aplicação | `http://+:8080` |
| `TZ` | Timezone da aplicação | `America/Sao_Paulo` |

### Configurações de Logging

| Variável | Descrição | Valor Padrão |
|----------|-----------|--------------|
| `Logging__LogLevel__Default` | Nível de log padrão | `Information` |
| `Logging__LogLevel__Microsoft.AspNetCore` | Nível de log do ASP.NET Core | `Warning` |

### Configurações do RabbitMQ

| Variável | Descrição | Valor Padrão | Sensível |
|----------|-----------|--------------|----------|
| `RabbitMq__Host` | Host do servidor RabbitMQ | `rabbitmq` | Não |
| `RabbitMq__Port` | Porta do servidor RabbitMQ | `5672` | Não |
| `RabbitMq__Username` | Usuário de conexão | `admin` | ⚠️ Sim |
| `RabbitMq__Password` | Senha de conexão | `rabbitmq123` | ⚠️ Sim |

> ⚠️ **Importante**: Em produção, utilize um gerenciador de secrets (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault) para armazenar credenciais sensíveis.

## 🚀 Como Executar

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [RabbitMQ](https://www.rabbitmq.com/) (ou via Docker)

### Executando Localmente

```bash
# Restaurar dependências
dotnet restore src/FiapCloudGames.Payments.API/FiapCloudGames.Payments.API.sln

# Executar a aplicação
dotnet run --project src/FiapCloudGames.Payments.API/FiapCloudGames.Payments.API.csproj
```

### Executando com Docker

```bash
# Build da imagem
docker build -t fiap-payments-api .

# Executar o container
docker run -d \
  -p 8080:8080 \
  -e RabbitMq__Host=rabbitmq \
  -e RabbitMq__Username=admin \
  -e RabbitMq__Password=rabbitmq123 \
  --name payments-api \
  fiap-payments-api
```

### Executando no Kubernetes

```bash
# Aplicar os manifests
kubectl apply -f k8s/
```

Consulte a documentação em [k8s/README.md](k8s/README.md) para mais detalhes sobre o deploy no Kubernetes.

## 📚 Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/health` | Health check da aplicação |
| GET | `/swagger` | Documentação Swagger da API |

## 🔄 Fluxo de Mensageria

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   Catalog API   │────▶│    RabbitMQ     │────▶│  Payments API   │
│                 │     │                 │     │                 │
│ Pedido Criado   │     │ pedido.criado   │     │ Processa        │
└─────────────────┘     └─────────────────┘     │ Pagamento       │
                                                └────────┬────────┘
                                                         │
                                                         ▼
                        ┌─────────────────┐     ┌─────────────────┐
                        │    RabbitMQ     │◀────│ Pagamento       │
                        │                 │     │ Processado      │
                        │ pagamento.      │     └─────────────────┘
                        │ processado      │
                        └─────────────────┘
```

## 📁 Estrutura do Projeto

```
.
├── src/
│   ├── FiapCloudGames.Payments.API/           # API Web
│   ├── FiapCloudGames.Payments.Application/   # Serviços e Use Cases
│   ├── FiapCloudGames.Payments.Domain/        # Entidades e Contratos
│   └── FiapCloudGames.Payments.Infrastructure/# Implementações
├── k8s/                                        # Manifests Kubernetes
├── Dockerfile                                  # Configuração Docker
├── .dockerignore                               # Arquivos ignorados no build
└── README.md                                   # Este arquivo
```

## 📝 Licença

Este projeto foi desenvolvido como parte do curso de pós-graduação da FIAP.