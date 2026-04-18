# TaskManager API

Uma API ASP.NET Core que implementa um serviço de enfileiramento de tarefas com processamento em background com suporte a pausa/retomada.

## 📋 Descrição

O TaskManager é um serviço que enfileira tarefas por meio de um endpoint POST e as processa de forma assíncrona em background usando múltiplos workers paralelos. A aplicação segue o padrão de separação de responsabilidades e oferece controle granular sobre o processamento.

## 🏗️ Arquitetura

### Separação de Responsabilidades

- **TaskQueueService**: Gerencia a fila de tarefas usando `Channel<T>` com suporte para múltiplos consumidores.
- **BackgroundTaskProcessor**: `BackgroundService` que processa tarefas da fila com múltiplos workers configuráveis.
- **PauseService**: Controla a pausa e retomada do processamento de tarefas.
- **TaskEndpoints**: Define os endpoints da API usando Minimal APIs.

### Fluxo de Dados

1. Cliente envia POST para `/api/tasks` com uma tarefa
2. A tarefa é enfileirada no `TaskQueueService` (Channel)
3. Os workers do `BackgroundTaskProcessor` consomem e processam as tarefas
4. `PauseService` controla se os workers estão ativos ou pausados

## 🚀 Endpoints

### POST `/api/tasks`
Enfileira uma nova tarefa para processamento.

**Request Body:**
```json
"sua tarefa aqui"
```

**Response:**
- `202 Accepted` - Tarefa aceita e enfileirada com sucesso
- `400 Bad Request` - Tarefa vazia ou inválida

**Exemplo:**
```bash
curl -X POST https://localhost:7170/api/tasks \
  -H "Content-Type: application/json" \
  -d '"Processar arquivo de dados"'
```

### POST `/api/pause`
Pausa o processamento de tarefas em background.

**Response:**
- `200 OK` - Processamento pausado

**Exemplo:**
```bash
curl -X POST https://localhost:7170/api/pause
```

**Comportamento:**
- As tarefas continuam sendo enfileiradas
- Os workers pausam antes de consumir a próxima tarefa
- Tarefas já em processamento continuam até finalizar

### POST `/api/resume`
Retoma o processamento de tarefas em background.

**Response:**
- `200 OK` - Processamento retomado

**Exemplo:**
```bash
curl -X POST https://localhost:7170/api/resume
```

**Comportamento:**
- Os workers começam a consumir tarefas da fila novamente
- A aplicação inicia com o processamento **pausado por padrão**

## ⚙️ Configuração

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "BackgroundTaskProcessor": {
    "WorkerCount": 2
  }
}
```

#### Propriedades

- **WorkerCount**: Número de workers paralelos que processam tarefas simultaneamente (padrão: 2)

## 🔧 Executar a Aplicação

### Pré-requisitos
- .NET 10 SDK
- Visual Studio Community 2026 ou VS Code

### Via Visual Studio
1. Abra o projeto em Visual Studio
2. Pressione `F5` para iniciar com debug
3. O navegador abrirá automaticamente na documentação Swagger

### Via CLI
```bash
cd TaskManager
dotnet run
```

A aplicação iniciará em:
- HTTPS: `https://localhost:7170`
- HTTP: `http://localhost:5030`

## 📚 Documentação da API

A documentação interativa da API está disponível em **Swagger UI**:

- **Development**: `https://localhost:7170/` (raiz)
- **Production**: Swagger desabilitado por segurança

### Teste os Endpoints no Swagger

1. Abra `https://localhost:7170/`
2. Expanda os endpoints
3. Clique em "Try it out" para testar

## 📊 Tecnologias Utilizadas

- **ASP.NET Core 10**
- **Minimal APIs**
- **Channel<T>** para fila assíncrona thread-safe
- **BackgroundService** para processamento em background
- **Swashbuckle/Swagger** para documentação
- **IOptions Pattern** para configurações

## 📁 Estrutura do Projeto

```
TaskManager/
├── Services/
│   ├── TaskQueueService.cs          # Gerencia a fila de tarefas
│   ├── BackgroundTaskProcessor.cs   # Processa tarefas em background
│   └── PauseService.cs              # Controla pausa/retomada
├── Endpoints/
│   └── TaskEndpoints.cs             # Define os endpoints da API
├── Options/
│   └── BackgroundTaskProcessorOptions.cs  # Configurações de opções
├── Program.cs                       # Configuração da aplicação
├── appsettings.json                 # Arquivo de configuração
└── README.md                        # Este arquivo
```

## 🔄 Fluxo de Exemplo

1. **Iniciar**: Aplicação inicia com processamento **pausado**
2. **Enfileirar**: `POST /api/tasks` com "Tarefa 1"
3. **Enfileirar**: `POST /api/tasks` com "Tarefa 2"
4. **Enfileirar**: `POST /api/tasks` com "Tarefa 3"
5. **Retomar**: `POST /api/resume`
   - Worker 1 processa "Tarefa 1"
   - Worker 2 processa "Tarefa 2" (em paralelo)
6. **Pausar**: `POST /api/pause`
   - Depois de finalizar as tarefas atuais, os workers param
7. **Enfileirar**: `POST /api/tasks` com "Tarefa 4"
   - Fica aguardando na fila (não é processada)
8. **Retomar**: `POST /api/resume`
   - Worker 1 processa "Tarefa 3"
   - Worker 2 processa "Tarefa 4"

## 📝 Logs

A aplicação registra informações sobre o processamento:

```
Worker 1 - 2026-04-18 10:30:45 - Tarefa 1
Worker 2 - 2026-04-18 10:30:46 - Tarefa 2
Worker 1 - 2026-04-18 10:30:47 - Tarefa 3
```

## 🎯 Próximas Melhorias

- Persistência de tarefas em banco de dados
- Retry automático em caso de falha
- Métricas e monitoramento
- Autenticação/Autorização nos endpoints
- Cancelamento de tarefas específicas

## 📄 Licença

Este projeto está disponível no GitHub: [jrochamartins/TaskManager](https://github.com/jrochamartins/TaskManager)