# Kubernetes Manifests - FIAP Payments API

Manifestos Kubernetes para deploy da Payments API no cluster.

## 📁 Arquivos

| Arquivo | Tipo | Descrição |
|---------|------|-----------|
| `configmap.yaml` | ConfigMap | Configurações **NÃO** sensíveis (env, URLs, RabbitMQ host) |
| `secret.yaml` | Secret | Dados **SENSÍVEIS** (credenciais RabbitMQ, API keys) |
| `deployment.yaml` | Deployment | Gerenciamento de Pods da API |
| `service.yaml` | Service | Exposição do serviço |

## 🚀 Deploy

### Via Orquestrador (Recomendado)

Use o repositório `fiap-orchestration` para deploy centralizado:

```bash
cd ../fiap-orchestration
.\scripts\deploy-all.ps1
```

### Deploy Individual

```bash
# 1. Criar namespace (se não existir)
kubectl create namespace fiap-cloud-games --dry-run=client -o yaml | kubectl apply -f -

# 2. Aplicar todos os manifestos
kubectl apply -f .

# 3. Verificar
kubectl get all -n fiap-cloud-games -l app=payments-api
```

## 🐰 Conexão com RabbitMQ

A API conecta ao RabbitMQ usando as configurações:

| Config | Valor | Fonte |
|--------|-------|-------|
| `RabbitMq:Host` | rabbitmq | ConfigMap |
| `RabbitMq:Port` | 5672 | ConfigMap |
| `RabbitMq:Username` | admin | Secret |
| `RabbitMq:Password` | *** | Secret |

## 📝 Convenções Seguidas

- ✅ **Deployments** para gerenciar Pods (não Pods isolados)
- ✅ **ConfigMaps** para configurações não sensíveis
- ✅ **Secrets** para dados sensíveis
- ✅ Namespace: `fiap-cloud-games`