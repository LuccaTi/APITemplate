# API Template — Web API em .NET 8

## Visão geral
Este repositório contém um template de API REST construída com ASP.NET Core (.NET 8). O projeto já vem estruturado com camadas simples (Controllers → Services → Repositories), injeção de dependência, logging com Serilog, configuração via `appsettings.json` e Swagger opcional para documentação e testes.

O propósito é criar um "esqueleto" que sirva como base para a criação de APIs que sigam um padrão tanto de implementação quanto de configuração e logging.

Endpoint de exemplo disponível:
- `GET /api/v1/test` → retorna uma lista de usuários da API de teste `jsonplaceholder.typicode.com`.
- `GET /api/v1/test/{id}` → retorna um usuário específico da lista de teste.

## Tecnologias e bibliotecas essenciais
- .NET 8 (ASP.NET Core)
- HttpClient: Biblioteca que faz as requisições e os tratamentos Http.
- Swashbuckle.AspNetCore (Swagger/OpenAPI)
- AutoMapper: Biblioteca usada para mapear os objetos consumidos em objetos criados pela API.
- Microsoft.Extensions.Configuration: Responsável por fazer leitura e escrita de arquivos de configuração.
- Serilog: Responsável por fazer a escrita em arquivos de Log.

## Estrutura do projeto
- `APITemplate.Host`:
    - **Responsibilidade**: É o projeto ponto de entrada executável (`.exe`). Ele monta e executa a aplicação.
    - **Contém**: Todas as pastas das classes usadas pela API além do Program.cs com a configuração essencial para iniciar a aplicação.


## Arquitetura e padrões de projeto
- Hospedagem e pipeline
    - Usa o `WebApplication` (minimal hosting) do ASP.NET Core.
    - Middlewares: ExceptionHandlerMiddleware, HTTPS redirection, Authorization (sem políticas ativas por padrão) e mapeamento de controllers.
    - Swagger/UI habilitado condicionalmente via configuração.

- Injeção de dependência
    - `IService.cs` → `Service.cs` registrado como `Scoped`.
    - Camadas separadas para facilitar testes e evolução.

- Logging (Serilog)
    - Logs em console e arquivo rolling diário em `logs/system_log_.txt`, a pasta com os logs fica no diretório base da aplicação.
    - Em falhas na inicialização, um arquivo é escrito pelo bootstrap logger para garantir rastreabilidade mesmo antes do logger oficial ser iniciado.

- Tratamento de erros
    - Exceções em endpoints são capturadas pelo ExceptionHandlerMiddleware que analisa o status code da requisição e retorna a resposta adequada ao cliente.

## Endpoints
- `GET /api/v1/Test`
    - Resposta 200: `Lista de users`
    - Resposta 400: detalhes do erro em caso de falha, com log registrado.
- `GET /api/v1/Test/{id}`
    - Resposta 200: `User específico`
    - Resposta 400: detalhes do erro em caso de falha, com log registrado.

## Configuração

### appsettings.json
Configurações principais da aplicação (seção `AppConfig`):
- `UseSwaggerProduction` ("true" | "false"): habilita/desabilita o uso do swagger em produção (conveniência para testes).
- `ClientUrl` (string): URL base que será consumida pela API.
- `Timeout` (int): Tempo de timeout das conexões.

### appsettings.Production.json
Configuração específica para ambiente de produção:
- Define o Kestrel para escutar HTTPS na porta 443 (padrão web).
- Usado quando `ASPNETCORE_ENVIRONMENT=Production`.

### Perfis de execução (local)
Definidos em `API.Host/Properties/launchSettings.json`:
- HTTPS: `https://localhost:5001`
- Ambiente padrão: `ASPNETCORE_ENVIRONMENT=Development`

## Uso da API
A API pode ser usada via console ao compilar o código e usar o .exe dentro do terminal, vale tanto para uso em Debug quanto Release.
