[![CI](https://github.com/<owner>/<repo>/actions/workflows/ci.yml/badge.svg)](https://github.com/<owner>/<repo>/actions/workflows/ci.yml)


# TaskManager API

A sample Task Manager API built with ASP.NET Core, EF Core, MediatR, AutoMapper, FluentValidation, Serilog, Identity, and JWT.

## Getting Started (Local)

### Prereqs
- .NET 8 SDK
- SQL Server LocalDB (or any SQL Server instance)

### Run locally
```bash
dotnet run --project src/TaskManager.Api


## Demo data

When the API starts with `SEED_DEMO=true`, it seeds:

- User: demo@task.local / Passw0rd!
- Project: "Demo Project"
- Task: "Your first task"
- Comment: "Welcome! This comment was seeded."

### Docker
`docker compose up -d` sets `SEED_DEMO=true` automatically.  
Open Swagger: http://localhost:5048/swagger  
Register/login or use the demo user to explore the endpoints.

