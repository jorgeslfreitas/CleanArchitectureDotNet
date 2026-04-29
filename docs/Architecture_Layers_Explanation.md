# Explicação das Camadas da Arquitetura

Este documento detalha cada camada da arquitetura do projeto Mostruário, incluindo propósito, responsabilidades e padrões específicos.

---

## Visão Geral

O projeto utiliza **arquitetura em camadas** (Layered Architecture) organizada da seguinte forma:

```
Mostruario.slnx
├── src/
│   ├── Mostruario.Domain/         # Camada de Domínio
│   ├── Mostruario.Application/    # Camada de Aplicação
│   ├── Mostruario.Infrastructure/ # Camada de Infraestrutura
│   └── Mostruario.Api/            # Camada de Apresentação
└── tests/
    └── Mostruario.Application.Tests/ # Testes
```

---

## Camada 1: Domain (Mostruario.Domain)

### Propósito

Contém as **regras de negócio fundamentais** e entidades do sistema. É a camada mais interna e não possui dependências externas.

### Responsabilidades

- Definir entidades de negócio
- Definir interfaces de repositório
- Containear exceções de negócio
- Encapsular validações e regras de negócio

### Componentes

#### 1.1 Entidades (Entities/)

Entidades representam objetos de negócio com identidade única.

**Exemplo: Category.cs**

```csharp
using Mostruario.Domain.Base;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Domain.Entities;

public class Category : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public Category() { }

    public Category(string name, string description)
    {
        Validate(name, description);
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Update(string name, string description)
    {
        Validate(name, description);
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessRuleException("Category name is required");
        // ... mais validações
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
```

**Padrões utilizados:**

- **Entidade com construtor privado sem parâmetro**: Para ORM
- **Construtor com parâmetros**: Para criação válida
- **Propriedades privadas com setters**: Imutabilidade controlada
- **Métodos de domínio**: Encapsulam comportamento
- **Validação no construtor**: Garante objetos válidos

#### 1.2 Classe Base (Base/)

**Exemplo: BaseEntity.cs**

```csharp
namespace Mostruario.Domain.Base;

public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public override bool Equals(object? obj) { /* implementação */ }
    public override int GetHashCode() { /* implementação */ }
    public static bool operator ==(...) { /* implementação */ }
    public static bool operator !=(...) { /* implementação */ }
}
```

**Propósito:**

- Fornece Id, CreatedAt, UpdatedAt para todas entidades
- Implementa equality por referência de Id

#### 1.3 Interfaces de Repositório (Interfaces/)

Definem contratos para acesso a dados.

**Exemplo: ICategoryRepository.cs**

```csharp
using Mostruario.Domain.Entities;

namespace Mostruario.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<(IEnumerable<Category> Items, int TotalCount)> GetAllAsync(int page, int pageSize, bool? isActive);
    Task<Category?> GetByIdWithProductsAsync(Guid id);
}
```

**IRepository.cs (base):**

```csharp
namespace Mostruario.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
```

#### 1.4 Exceções (Exceptions/)

**BusinessRuleException.cs**

```csharp
namespace Mostruario.Domain.Exceptions;

public class BusinessRuleException(string message) : Exception(message) { }
```

**NotFoundException.cs**

```csharp
namespace Mostruario.Domain.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' not found") { }
}
```

---

## Camada 2: Application (Mostruario.Application)

### Propósito

Contém a **lógica de aplicação** e casos de uso. Orquestra o fluxo de dados entre a camada de domínio e apresentação.

### Responsabilidades

- Implementar Use Cases
- Definir DTOs
- Criar mapeamentos Entidade ↔ DTO
- Definir interfaces de Use Cases

### Componentes

#### 2.1 DTOs (Data Transfer Objects)

Objetos para transferência de dados entre camadas.

**Exemplos:**

```csharp
// DTO de resposta completa
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

// DTO para criação
public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

// DTO para paginação
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
```

#### 2.2 Mapeamentos (Mappings/)

Converte entre Entidades e DTOs.

**Exemplo: CategoryMapper.cs**

```csharp
using Mostruario.Domain.Entities;
using CategoryDto = Mostruario.Application.DTOs.CategoryDto;
using CreateCategoryDto = Mostruario.Application.DTOs.CreateCategoryDto;
using UpdateCategoryDto = Mostruario.Application.DTOs.UpdateCategoryDto;

namespace Mostruario.Application.Mappings;

public static class CategoryMapper
{
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = category.Products?.Count ?? 0,
            CreatedAt = category.CreatedAt
        };
    }

    public static Category ToEntity(CreateCategoryDto dto)
    {
        return new Category(dto.Name, dto.Description);
    }

    public static void UpdateEntity(Category category, UpdateCategoryDto dto)
    {
        category.Update(dto.Name, dto.Description);
        
        if (dto.IsActive != category.IsActive)
        {
            if (dto.IsActive) category.Activate();
            else category.Deactivate();
        }
    }
}
```

#### 2.3 Interfaces de Use Cases (Interfaces/)

Contratos para as operações de aplicação.

**Exemplo: ICategoryUseCases.cs**

```csharp
using Mostruario.Application.DTOs;

namespace Mostruario.Application.Interfaces.Categories;

public interface IGetAllCategoriesUseCase
{
    Task<PagedResult<CategoryDto>> ExecuteAsync(int page = 1, int pageSize = 10, bool? isActive = null);
}

public interface IGetCategoryByIdUseCase
{
    Task<CategoryDto> ExecuteAsync(Guid id);
}

public interface ICreateCategoryUseCase
{
    Task<CategoryDto> ExecuteAsync(CreateCategoryDto dto);
}

public interface IUpdateCategoryUseCase
{
    Task<CategoryDto> ExecuteAsync(UpdateCategoryDto dto);
}

public interface IDeleteCategoryUseCase
{
    Task ExecuteAsync(Guid id);
}
```

#### 2.4 Implementação de Use Cases

Lógica de aplicação que orquestra o fluxo.

**Exemplo: CreateCategoryUseCase.cs**

```csharp
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Categories;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Categories;

public class CreateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : ICreateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CategoryDto> ExecuteAsync(CreateCategoryDto dto)
    {
        var category = CategoryMapper.ToEntity(dto);
        
        await _categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return CategoryMapper.ToDto(category);
    }
}
```

**Exemplo: UpdateCategoryUseCase.cs (com validação)**

```csharp
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Categories;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Categories;

public class UpdateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork)
    : IUpdateCategoryUseCase
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CategoryDto> ExecuteAsync(UpdateCategoryDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(dto.Id);
        
        if (category == null)
            throw new NotFoundException("Category", dto.Id);

        CategoryMapper.UpdateEntity(category, dto);
        
        await _categoryRepository.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return CategoryMapper.ToDto(category);
    }
}
```

---

## Camada 3: API (Mostruario.Api)

### Propósito

Expor a aplicação via HTTP e configurar o host.

### Responsabilidades

- Controladores HTTP
- Configuração de injeção de dependência
- Middlewares
- Autenticação e autorização

### Componentes

#### 3.1 Controllers

Exponhem endpoints REST.

**Exemplo: CategoriesController.cs**

```csharp
using Microsoft.AspNetCore.Mvc;
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Categories;
using Microsoft.AspNetCore.Authorization;

namespace Mostruario.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoriesController(
    IGetAllCategoriesUseCase getAllUseCase,
    IGetCategoryByIdUseCase getByIdUseCase,
    ICreateCategoryUseCase createUseCase,
    IUpdateCategoryUseCase updateUseCase,
    IDeleteCategoryUseCase deleteUseCase)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<CategoryDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        var result = await getAllUseCase.ExecuteAsync(page, pageSize, isActive);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var result = await getByIdUseCase.ExecuteAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await createUseCase.ExecuteAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        dto.Id = id;
        var result = await updateUseCase.ExecuteAsync(dto);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        await deleteUseCase.ExecuteAsync(id);
        return NoContent();
    }
}
```

**Padrões utilizados:**

- **[ApiController]**: Ativa recursos automáticos de API
- **[Route("api/v1/[controller]")]**: Naming convention REST
- **[AllowAnonymous]**: Endpoint público
- **[Authorize]**: Endpoint requer autenticação
- **Injeção de dependência via construtor**: Use Cases injetados

#### 3.2 Configuração de DI (Extensions/)

**DependencyInjection.cs**

```csharp
using System.Text;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApiDependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddFluentValidationAutoValidation();
        // Swagger config...
        return services;
    }

    public static IServiceCollection AddApiAuthentication(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("JWT SecretKey not configured");
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "MostruarioApi",
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSecret))
                };
            });

        services.AddAuthorization();
        return services;
    }
}
```

#### 3.3 Middleware

**ErrorHandlingMiddleware**: Tratamento global de exceções.

#### 3.4 Program.cs

```csharp
using Mostruario.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddApiAuthentication(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

## Camada 4: Tests (Mostruario.Application.Tests)

### Propósito

Garantir qualidade através de testes unitários.

### Responsabilidades

- Testar Use Cases
- Validar regras de negócio
- Garantir comportamento esperado

### Estrutura

```
tests/Mostruario.Application.Tests/
├── UseCases/
│   ├── CreateCategoryUseCaseTests.cs
│   ├── CreateProductUseCaseTests.cs
│   └── ...
```

### Exemplo de Teste

```csharp
using FluentAssertions;
using Mostruario.Application.DTOs;
using Mostruario.Application.UseCases.Categories;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;
using Moq;
using Xunit;

namespace Mostruario.Application.Tests.UseCases;

public class CreateCategoryUseCaseTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateCategoryUseCase _useCase;

    public CreateCategoryUseCaseTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new CreateCategoryUseCase(
            _categoryRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_ReturnsBrandDto()
    {
        // Arrange
        var dto = new CreateCategoryDto
        {
            Name = "Eletrônicos",
            Description = "Produtos eletrônicos"
        };

        _categoryRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Category>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        _categoryRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<Category>()), 
            Times.Once);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(), 
            Times.Once);
    }
}
```

**Frameworks utilizados:**

- **xUnit**: Framework de testes
- **Moq**: Framework de mocking
- **FluentAssertions**: Assertions fluentes

---

## Fluxo de Dados

```
HTTP Request
    ↓
[Controller]
    ↓ (injeção de dependência)
[Use Case]
    ↓ (chama interface)
[Repository Interface]
    ↓ (implementação)
[Infrastructure]
    ↓ (acesso a dados)
Database
```

---

## Padrões de Projeto Utilizados

| Padrão | Aplicação |
|--------|-----------|
| **Repository** | IRepository<T> abstrai acesso a dados |
| **Unit of Work** | IUnitOfWork gerencia transações |
| **Use Case** | Cada operação é um Use Case separado |
| **DTO** | Objetos para transferência entre camadas |
| **Mapper** | Converte Entidade ↔ DTO |
| **Dependency Injection** | Injeção via construtor |
| **Fluent Interface** | Mapeamentos estáticos encadeados |

---

## Dependências entre Camadas

```
Api → Application → Domain
```

- **Domain**: Não depende de nenhuma camada
- **Application**: Depende apenas de Domain
- **Api**: Depende de Application e Domain
- **Tests**: Depende de Application e Domain

Esta organização segue o **Princípio de Dependência** do SOLID: módulos de alto nível não devem depender de módulos de baixo nível.
