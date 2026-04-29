# Referência de Comandos dotnet CLI

Referência completa dos comandos mais utilizados para o projeto Mostruário.

---

## 1. Comandos de Projeto e Solução

### 1.1 Criação

```bash
# Criar solução
dotnet new sln -n Mostruario

# Criar projetos
dotnet new webapi -n Mostruario.Api -o src/Mostruario.Api
dotnet new classlib -n Mostruario.Domain -o src/Mostruario.Domain
dotnet new classlib -n Mostruario.Application -o src/Mostruario.Application
dotnet new classlib -n Mostruario.Infrastructure -o src/Mostruario.Infrastructure
dotnet new xunit -n Mostruario.Application.Tests -o tests/Mostruario.Application.Tests
```

### 1.2 Gerenciamento de Solução

```bash
# Listar projetos na solução
dotnet sln list

# Adicionar projeto à solução
dotnet sln add src/Mostruario.Api/Mostruario.Api.csproj
dotnet sln add src/Mostruario.Domain/Mostruario.Domain.csproj

# Remover projeto da solução
dotnet sln remove src/Mostruario.Api/Mostruario.Api.csproj

# Listar projetos referenciados
dotnet sln Mostruario.slnx reference list
```

---

## 2. Referências de Projeto

### 2.1 Adicionar referências

```bash
# Adicionar referência de projeto
dotnet add src/Mostruario.Application/Mostruario.Application.csproj reference src/Mostruario.Domain/Mostruario.Domain.csproj

# Adicionar múltiplas referências
dotnet add app.csproj reference lib1.csproj lib2.csproj
```

### 2.2 Listar referências

```bash
# Listar referências de um projeto
dotnet list src/Mostruario.Api/Mostruario.Api.csproj reference
```

### 2.3 Remover referências

```bash
# Remover referência
dotnet remove src/Mostruario.Api/Mostruario.Api.csproj reference src/Mostruario.Domain/Mostruario.Domain.csproj
```

---

## 3. Pacotes NuGet

### 3.1 Adicionar pacotes

```bash
# Adicionar pacote por nome
dotnet add <CAMINHO_DO_PROJETO> package <NOME_DO_PACOTE>
dotnet add package FluentValidation.AspNetCore
dotnet add package Moq
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Adicionar pacote específico com versão
dotnet add package Newtonsoft.Json --version 13.0.1

# Adicionar sem prompts (para scripts)
dotnet add package Newtonsoft.Json -v 13.0.1 --no-restore
```

### 3.2 Listar pacotes

```bash
# Listar pacotes instalados
dotnet list src/Mostruario.Api/Mostruario.Api.csproj package
```

### 3.3 Remover pacotes

```bash
# Remover pacote
dotnet remove package Newtonsoft.Json
```

---

## 4. Build e Execução

### 4.1 Build

```bash
# Compilar solução
dotnet build

# Compilar projeto específico
dotnet build src/Mostruario.Api/Mostruario.Api.csproj

# Build em Release
dotnet build -c Release

# Build sem dependências (mais rápido para desenvolvimento)
dotnet build --no-dependencies

# Build com número específico de CPUs
dotnet build -m 4

# Verbose output
dotnet build -v detailed
```

### 4.2 Execução

```bash
# Executar projeto padrão
dotnet run

# Executar projeto específico
dotnet run --project src/Mostruario.Api/Mostruario.Api.csproj

# Executar com argumentos
dotnet run -- --arg1 value1

# Executar em porta específica
dotnet run --urls "http://localhost:5000;https://localhost:5001"

# Executar em modo Release
dotnet run -c Release

# Hot reload (desenvolvimento)
dotnet watch run --project src/Mostruario.Api/Mostruario.Api.csproj
```

### 4.3 Publicação

```bash
# Publicar para pasta
dotnet publish -o ./publish

# Publicar self-contained
dotnet publish -c Release -r win-x64 --self-contained true

# Publicar para Linux
dotnet publish -c Release -r linux-x64 --self-contained true

# Publicar com runtime mínimo
dotnet publish -c Release --self-contained true -p:PublishSingleFile=true
```

---

## 5. Testes

### 5.1 Execução

```bash
# Executar todos os testes
dotnet test

# Executar projeto de testes específico
dotnet test tests/Mostruario.Application.Tests/Mostruario.Application.Tests.csproj

# Executar testes em modo Release
dotnet test -c Release

# Executar com saída detalhada
dotnet test -v detailed

# Executar com output de console
dotnet test --logger "console;verbosity=detailed"
```

### 5.2 Filtragem

```bash
# Executar teste específico por nome
dotnet test --filter "FullyQualifiedName~CreateCategoryUseCaseTests"

# Executar testes de uma classe
dotnet test --filter "FullyQualifiedName~CreateCategoryUseCaseTests"

# Executar testes com trait
dotnet test --filter "Category=UnitTest"

# Executar múltiplos filtros (OR)
dotnet test --filter "FullyQualifiedName~Create&FullyQualifiedName~Update"

# Executar múltiplos filtros (AND)
dotnet test --filter "FullyQualifiedName~Category&FullyQualifiedName~Valid"
```

### 5.3 Cobertura

```bash
# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Gerar relatório de cobertura
dotnet test --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollectorSettings.Format=opencover
```

### 5.4 Opções de parallelismo

```bash
# Executar testes em paralelo
dotnet test --parallel

# Limitar paralelismo
dotnet test --parallel --maxcpucount 2
```

---

## 6. Limpeza e Manutenção

### 6.1 Limpeza

```bash
# Limpar solução
dotnet clean

# Limpar projeto específico
dotnet clean src/Mostruario.Api/Mostruario.Api.csproj

# Limpar em Release
dotnet clean -c Release
```

### 6.2 Restore

```bash
# Restaurar dependências
dotnet restore

# Restaurar com source específico
dotnet restore --source https://api.nuget.org/v3/index.json

# Restaurar offline
dotnet restore --offline
```

---

## 7. Scripts de Projeto

### 7.1 Criar script de build

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

### 7.2 Configurar testes no .csproj

```xml
<!-- tests/Mostruario.Application.Tests/Mostruario.Application.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.4">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="FluentAssertions" Version="6.12.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\src\Mostruario.Application\Mostruario.Application.csproj" />
    <ProjectReference Include="..\..\src\Mostruario.Domain\Mostruario.Domain.csproj" />
  </ItemGroup>
</Project>
```

---

## 8. Atalhos e Configurações Úteis

### 8.1 Configurar projeto padrão

```json
// global.json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestMinor"
  }
}
```

### 8.2 Configurações comuns de build

```bash
# Definir configuração padrão
dotnet build -c Debug

# Build com warnings como errors
dotnet build -p:TreatWarningsAsErrors=true

# Suprimir warnings específicos
dotnet build -p:NoWarn=CS8618
```

---

## 9. Comandos de Diagnóstico

### 9.1 Informações do SDK

```bash
# Ver versão do SDK
dotnet --version

# Listar SDKs instalados
dotnet --list-sdks

# Ver informações detalhadas
dotnet --info
```

### 9.2 Cache e locais

```bash
# Listar locais NuGet
dotnet nuget locals all --list

# Limpar cache
dotnet nuget locals all --clear
```

---

## 10. Tabela Resumo

| Comando | Descrição |
|---------|-----------|
| `dotnet new sln` | Criar solução |
| `dotnet new webapi` | Criar API |
| `dotnet new classlib` | Criar biblioteca |
| `dotnet sln add` | Adicionar à solução |
| `dotnet add reference` | Adicionar referência |
| `dotnet add package` | Adicionar NuGet |
| `dotnet build` | Compilar |
| `dotnet run` | Executar |
| `dotnet watch run` | Hot reload |
| `dotnet test` | Executar testes |
| `dotnet clean` | Limpar build |
| `dotnet restore` | Restaurar |
| `dotnet publish` | Publicar |

---

## 11. Troubleshooting

### Problemas comuns

```bash
# Dependências corrompidas
dotnet restore --force

# Conflictos de versão
dotnet restore --verbosity detailed

# Lock em arquivos
dotnet build --force

# Limpar completamente
rm -rf **/bin **/obj
dotnet restore
dotnet build
```
