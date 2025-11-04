# API Template — Web API em .NET 8

## Visão geral
Este repositório contém um template de API REST construída com ASP.NET Core (.NET 8). O projeto já vem estruturado com camadas simples (Controllers → Services → Repositories), injeção de dependência, logging com Serilog, configuração via `appsettings.json` e Swagger opcional para documentação e testes.

O propósito é criar um "esqueleto" que sirva como base para a criação de APIs que sigam um padrão tanto de implementação quanto de configuração e logging.

Endpoint de exemplo disponível:
- `GET /api/v1/test` → retorna `{ "message": "API is Working!" }`.

## Tecnologias e bibliotecas essenciais
- .NET 8 (ASP.NET Core)
- Serilog.AspNetCore (logging para console e arquivo)
- Swashbuckle.AspNetCore (Swagger/OpenAPI)

## Estrutura do projeto
- `API.Host/Program.cs`: ponto de entrada; configura DI, logging, controllers, Swagger e middlewares.
- `API.Host/Controllers/`: controladores HTTP (ex.: `TestController.cs`).
- `API.Host/Interfaces/`: contratos de serviços (ex.: `IApiService.cs`).
- `API.Host/Services/`: implementação da regra de negócio (ex.: `ApiService.cs`).
- `API.Host/Repositories/`: camada de acesso a dados (placeholder para futura implementação).
- `API.Host/Logging/Logger.cs`: inicialização e wrappers do Serilog.
- `API.Host/appsettings.json`: configurações da aplicação (Swagger, diretório de logs, níveis de log).
- `API.Host/Properties/launchSettings.json`: perfis de execução locais (portas e ambiente).

## Arquitetura e padrões de projeto
- Hospedagem e pipeline
    - Usa o `WebApplication` (minimal hosting) do ASP.NET Core.
    - Middlewares: HTTPS redirection, Authorization (sem políticas ativas por padrão) e mapeamento de controllers.
    - Swagger/UI habilitado condicionalmente via configuração.

- Injeção de dependência
    - `IApiService` → `ApiService` registrado como `Scoped`.
    - Camadas separadas para facilitar testes e evolução.

- Logging (Serilog)
    - Logs em console e arquivo rolling diário em `logs/system_log_.txt` (diretório configurável).
    - Em falhas na inicialização, um arquivo é escrito em `StartupErrors/` para garantir rastreabilidade mesmo antes do logger.

- Tratamento de erros
    - Exceções em endpoints são capturadas no controller e logadas antes de retornar `400 Bad Request`.

## Endpoints
- `GET /api/v1/test`
    - Resposta 200: `{ "message": "API is Working!" }`
    - Resposta 400: detalhes do erro em caso de falha, com log registrado.

## Configuração

### appsettings.json
Configurações principais da aplicação (seção `API`):
- `UseSwagger` ("true" | "false"): habilita/desabilita o uso do swagger.
- `LogDirectory` (string): pasta para gravação dos logs (relativa ao diretório base da aplicação).

### appsettings.Production.json
Configuração específica para ambiente de produção:
- Define o Kestrel para escutar HTTPS na porta 443 (padrão web).
- Usado quando `ASPNETCORE_ENVIRONMENT=Production`.

### Perfis de execução (local)
Definidos em `API.Host/Properties/launchSettings.json`:
- HTTPS: `https://localhost:5001`
- Ambiente padrão: `ASPNETCORE_ENVIRONMENT=Development`

## Uso da API
A API pode ser usada via console ao compilar o código e usar o .exe dentro do terminal, dependendo da configuração do appsettings.json vai abrir ou não o swagger no navegador padrão da máquina, quando usada para desenvolvimento (Debug, IDE) também vai abrir automaticamente o navegador.
