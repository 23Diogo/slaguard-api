# SlaGuard API

API em ASP.NET Core para registro e acompanhamento de incidentes, prioridades e prazos de SLA.

## O que o projeto demonstra

- ASP.NET Core Web API e C#
- Minimal APIs e injeção de dependência
- Regras de negócio para cálculo e acompanhamento de SLA
- Filtros por status, prioridade e texto
- OpenAPI
- Testes unitários com xUnit
- Pipeline de build e testes no GitHub Actions

## Executar

```bash
dotnet restore
dotnet run --project src/SlaGuard.Api
```

A documentação OpenAPI fica disponível em `/openapi/v1.json`.

## Testes

```bash
dotnet test
```

> Projeto de laboratório criado para demonstrar conhecimentos em .NET e desenvolvimento de APIs.
