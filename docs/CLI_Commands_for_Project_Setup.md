# Comandos CLI para Configuração de Projeto

Este guia apresenta os comandos CLI para criar e configurar um novo projeto .NET similar ao Mostruário.

---

## Pré-requisitos

- .NET 8.0 SDK instalado
- Rider, VS Code ou Visual Studio

---

## 1. Criar Estrutura de Solução

### 1.1 Criar arquivo de solução (formato moderno .slnx)

```bash
# Criar solução vazia
dotnet new sln -n Mostruario

# Listar soluções existentes
dotnet sln list
```

### 1.2 Criar projetos

```bash
# Criar projeto de API (ASP.NET Core Web API)
dotnet new webapi -n Mostruario.Api -o src/Mostruario.Api

# Criar projeto de biblioteca de classes (Domain)
dotnet new classlib -n Mostruario.Domain -o src/Mostruario.Domain

# Criar projeto de biblioteca de classes (Application)
dotnet new classlib -n Mostruario.Application -o src/Mostruario.Application

# Criar projeto de biblioteca de classes (Infrastructure)
dotnet new classlib -n Mostruario.Infrastructure -o src/Mostruario.Infrastructure

# Criar projeto de testes (xUnit)
dotnet new xunit -n Mostruario.Application.Tests -o tests/Mostruario.Application.Tests
```

### 1.3 Vincular projetos à solução

```bash
# Adicionar todos os projetos de uma vez
dotnet sln add src/Mostruario.Api/Mostruario.Api.csproj
dotnet sln add src/Mostruario.Domain/Mostruario.Domain.csproj
dotnet sln add src/Mostruario.Application/Mostruario.Application.csproj
dotnet sln add src/Mostruario.Infrastructure/Mostruario.Infrastructure.csproj
dotnet sln add tests/Mostruario.Application.Tests/Mostruario.Application.Tests.csproj

# Adicionar todos os projetos (PowerShell)
Get-ChildItem -Filter *.csproj -r | ForEach-Object { dotnet sln add $_.FullName }
```

---

## 2. Configurar Referências entre Projetos

### 2.1 Adicionar referências de projeto

```bash
# Application referencia Domain
dotnet add src/Mostruario.Application/Mostruario.Application.csproj reference src/Mostruario.Domain/Mostruario.Domain.csproj

# Api referencia Application e Domain
dotnet add src/Mostruario.Api/Mostruario.Api.csproj reference src/Mostruario.Application/Mostruario.Application.csproj
dotnet add src/Mostruario.Api/Mostruario.Api.csproj reference src/Mostruario.Domain/Mostruario.Domain.csproj

# Tests referencia Application
dotnet add tests/Mostruario.Application.Tests/Mostruario.Application.Tests.csproj reference src/Mostruario.Application/Mostruario.Application.csproj

# Verificar referências
dotnet list src/Mostruario.Api/Mostruario.Api.csproj reference
```

---

## 3. Adicionar Pacotes NuGet

### 3.1 Pacotes essenciais

```bash
# API - Swagger e JWT
dotnet add src/Mostruario.Api/Mostruario.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/Mostruario.Api/Mostruario.Api.csproj package Swashbuckle.AspNetCore

# Application - FluentValidation
dotnet add src/Mostruario.Application/Mostruario.Application.csproj package FluentValidation.AspNetCore

# Domain - (geralmente sem dependências externas)
```

### 3.2 Pacotes de testes

```bash
# Tests - Moq e FluentAssertions
dotnet add tests/Mostruario.Application.Tests/Mostruario.Application.Tests.csproj package Moq
dotnet add tests/Mostruario.Application.Tests/Mostruario.Application.Tests.csproj package FluentAssertions
```

---

## 4. Criar Estrutura de Pastas

### 4.1 Criar pastas no Domain

```bash
# Usando PowerShell no Windows
mkdir src/Mostruario.Domain/Entities
mkdir src/Mostruario.Domain/Interfaces
mkdir src/Mostruario.Domain/Base
mkdir src/Mostruario.Domain/Exceptions
mkdir src/Mostruario.Domain/Enums
```

### 4.2 Criar pastas no Application

```bash
mkdir src/Mostruario.Application/DTOs
mkdir src/Mostruario.Application/Interfaces/Categories
mkdir src/Mostruario.Application/UseCases/Categories
mkdir src/Mostruario.Application/Mappings
mkdir src/Mostruario.Application/Validators
```

### 4.3 Criar pastas no Api

```bash
mkdir src/Mostruario.Api/Controllers
mkdir src/Mostruario.Api/Middleware
mkdir src/Mostruario.Api/Filters
mkdir src/Mostruario.Api/Extensions
```

### 4.4 Criar pastas nos Tests

```bash
mkdir tests/Mostruario.Application.Tests/UseCases
```

---

## 5. Criar Classes via CLI

### 5.1 Criar entidade

```bash
# Criar arquivo de entidade
dotnet new class -n Category -o src/Mostruario.Domain/Entities

# Ou simplesmente criar o arquivo manualmente
```

### 5.2 Criar arquivo vazio para use case

```bash
# Criar use case (requer estrutura manual posterior)
dotnet new class -n CreateCategoryUseCase -o src/Mostruario.Application/UseCases/Categories
```

---

## 6. Comandos de Build e Execução

### 6.1 Restaurar dependências

```bash
dotnet restore
```

### 6.2 Compilar projeto

```bash
# Compilar solução completa
dotnet build

# Compilar projeto específico
dotnet build src/Mostruario.Api/Mostruario.Api.csproj

# Build em modo Release
dotnet build -c Release
```

### 6.3 Executar projeto

```bash
# Executar API (a partir do diretório do projeto)
dotnet run --project src/Mostruario.Api/Mostruario.Api.csproj

# Executar em porta específica
dotnet run --urls "http://localhost:5000"

# Executar com hot reload
dotnet watch run --project src/Mostruario.Api/Mostruario.Api.csproj
```

---

## 7. Comandos de Testes

### 7.1 Executar testes

```bash
# Executar todos os testes
dotnet test

# Executar testes de projeto específico
dotnet test tests/Mostruario.Application.Tests/Mostruario.Application.Tests.csproj

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### 7.2 Opções de teste

```bash
# Executar teste específico por nome
dotnet test --filter "FullyQualifiedName~CreateCategoryUseCaseTests"

# Executar em paralelo
dotnet test --parallel

# Verbose output
dotnet test -v detailed
```

---

## 8. Comandos de Limpeza

### 8.1 Limpar build

```bash
# Limpar bin e obj
dotnet clean

# Limpar solução completa
dotnet clean Mostruario.slnx
```

### 8.2 Remover dependências não utilizadas

```bash
dotnet nuget locals all --clear
```

---

## 9. Scripts Úteis

### 9.1 Script PowerShell: Criar estrutura completa

```powershell
# criar_estrutura.ps1

$slnName = "Mostruario"
$srcPath = "src"
$testPath = "tests"

# Criar diretórios
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Domain/Entities"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Domain/Interfaces"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Domain/Base"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Domain/Exceptions"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Application/DTOs"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Application/Interfaces"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Application/UseCases"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Application/Mappings"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Api/Controllers"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Api/Middleware"
New-Item -ItemType Directory -Force -Path "$srcPath/Mostruario.Api/Extensions"
New-Item -ItemType Directory -Force -Path "$testPath/Mostruario.Application.Tests/UseCases"

Write-Host "Estrutura criada com sucesso!"
```

### 9.2 Script Bash: Criar estrutura (Linux/Mac)

```bash
#!/bin/bash

mkdir -p src/Mostruario.Domain/{Entities,Interfaces,Base,Exceptions,Enums}
mkdir -p src/Mostruario.Application/{DTOs,Interfaces,UseCases,Mappings,Validators}
mkdir -p src/Mostruario.Infrastructure/{Data,Repositories}
mkdir -p src/Mostruario.Api/{Controllers,Middleware,Filters,Extensions}
mkdir -p tests/Mostruario.Application.Tests/UseCases

echo "Estrutura criada com sucesso!"
```

---

## 10. Comandos do Rider/VS Code

### 10.1 Rider

O JetBrains Rider detecta automaticamente arquivos .slnx e configura o projeto.

### 10.2 VS Code

```json
// .vscode/tasks.json - Configurar build
{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build",
            "command": "dotnet",
            "type": "process",
            "args": ["build"],
            "problemMatcher": "$msCompile"
        },
        {
            "label": "test",
            "command": "dotnet",
            "type": "process",
            "args": ["test"],
            "problemMatcher": "$msCompile"
        }
    ]
}
```

---

## Resumo de Comandos

| Comando | Descrição |
|---------|-----------|
| `dotnet new sln -n Nome` | Criar solução |
| `dotnet new webapi -n Nome` | Criar projeto API |
| `dotnet new classlib -n Nome` | Criar biblioteca |
| `dotnet new xunit -n Nome` | Criar projeto de testes |
| `dotnet sln add` | Adicionar projeto à solução |
| `dotnet add reference` | Adicionar referência |
| `dotnet add package` | Adicionar NuGet |
| `dotnet build` | Compilar |
| `dotnet run` | Executar |
| `dotnet test` | Executar testes |
| `dotnet clean` | Limpar build |
| `dotnet restore` | Restaurar dependências |
