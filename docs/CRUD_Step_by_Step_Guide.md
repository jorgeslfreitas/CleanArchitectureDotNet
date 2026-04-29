# Guia Passo a Passo: Criando um CRUD Completo

Este guia detalha cada etapa para implementar um novo CRUD completo no projeto Mostruário, usando como exemplo prático a implementação de **Marca de Produtos** (Brand).

---

## Visão Geral da Arquitetura

O projeto segue uma arquitetura em camadas (Layered Architecture):

```
Mostruario.slnx
├── src/
│   ├── Mostruario.Domain/        # Entidades, interfaces de repositório, exceções
│   ├── Mostruario.Application/    # Use Cases, DTOs, Mapeamentos, validadores
│   ├── Mostruario.Infrastructure/ # Implementação de repositórios (futuro)
│   └── Mostruario.Api/            # Controllers, middlewares, configurações
└── tests/
    └── Mostruario.Application.Tests/
```

Cada camada tem responsabilidade específica:

- **Domain**: Entidades de negócio e interfaces de acesso a dados
- **Application**: Lógica de aplicação (Use Cases), DTOs e mapeamentos
- **Api**: Controladores HTTP e configuração de injeção de dependência

---

## Etapa 1: Criar a Entidade no Domínio

### Objetivo

Definir a entidade de negócio que representa o recurso.

### Localização

`src/Mostruario.Domain/Entities/Brand.cs`

### Implementação

```csharp
using Mostruario.Domain.Base;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Domain.Entities;

public class Brand : BaseEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    private readonly List<Product> _products = new();
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    public Brand() { }

    public Brand(string name, string description)
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
            throw new BusinessRuleException("Brand name is required");

        if (name.Length > 100)
            throw new BusinessRuleException("Brand name must be at most 100 characters");

        if (description.Length > 500)
            throw new BusinessRuleException("Brand description must be at most 500 characters");
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Explicação

- **herança de BaseEntity<Guid>**: Fornece Id, CreatedAt, UpdatedAt automaticamente
- **construtor parametrizado**: Cria instância com validação de negócio
- **métodos de domínio**: Activate(), Deactivate(), Update() encapsulam lógica de negócio
- **validação estática**: Garante que entidades inválidas nunca sejam criadas

---

## Etapa 2: Criar a Interface do Repositório

### Objetivo

Definir o contrato para acesso a dados da entidade.

### Localização

`src/Mostruario.Domain/Interfaces/IBrandRepository.cs`

### Implementação

```csharp
using Mostruario.Domain.Entities;

namespace Mostruario.Domain.Interfaces;

public interface IBrandRepository : IRepository<Brand>
{
    Task<(IEnumerable<Brand> Items, int TotalCount)> GetAllAsync(int page, int pageSize, bool? isActive);
    Task<Brand?> GetByIdWithProductsAsync(Guid id);
}
```

### Explicação

- **herança de IRepository<Brand>**: Herda operações base (GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync)
- **métodos específicos**: GetAllAsync com paginação e GetByIdWithProductsAsync para relacionamentos

---

## Etapa 3: Criar os DTOs

### Objetivo

Definir os objetos de transferência de dados para a camada de aplicação.

### Localização

`src/Mostruario.Application/DTOs/`

### 3.1 BrandDto.cs (Resposta)

```csharp
namespace Mostruario.Application.DTOs;

public class BrandDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### 3.2 CreateBrandDto.cs (Criação)

```csharp
namespace Mostruario.Application.DTOs;

public class CreateBrandDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

### 3.3 UpdateBrandDto.cs (Atualização)

```csharp
namespace Mostruario.Application.DTOs;

public class UpdateBrandDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
```

### Explicação

- **BrandDto**: Resposta completa incluindo metadados
- **CreateBrandDto**: apenas campos necessários para criação (sem Id)
- **UpdateBrandDto**: Todos os campos editáveis incluindo Id

---

## Etapa 4: Criar o Mapper

### Objetivo

Definir conversões entre Entidade e DTOs.

### Localização

`src/Mostruario.Application/Mappings/BrandMapper.cs`

### Implementação

```csharp
using Mostruario.Domain.Entities;
using BrandDto = Mostruario.Application.DTOs.BrandDto;
using CreateBrandDto = Mostruario.Application.DTOs.CreateBrandDto;
using UpdateBrandDto = Mostruario.Application.DTOs.UpdateBrandDto;

namespace Mostruario.Application.Mappings;

public static class BrandMapper
{
    public static BrandDto ToDto(Brand brand)
    {
        return new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description,
            IsActive = brand.IsActive,
            ProductCount = brand.Products?.Count ?? 0,
            CreatedAt = brand.CreatedAt
        };
    }

    public static Brand ToEntity(CreateBrandDto dto)
    {
        return new Brand(dto.Name, dto.Description);
    }

    public static void UpdateEntity(Brand brand, UpdateBrandDto dto)
    {
        brand.Update(dto.Name, dto.Description);

        if (dto.IsActive != brand.IsActive)
        {
            if (dto.IsActive)
                brand.Activate();
            else
                brand.Deactivate();
        }
    }
}
```

### Explicação

- **ToDto**: Converte Entidade → DTO de resposta
- **ToEntity**: Converte DTO de criação → Entidade (usa construtor com validação)
- **UpdateEntity**: Aplica alterações do DTO à entidade existente

---

## Etapa 5: Criar as Interfaces dos Use Cases

### Objetivo

Definir contratos para as operações de aplicação.

### Localização

`src/Mostruario.Application/Interfaces/Brands/IBrandUseCases.cs`

### Implementação

```csharp
using Mostruario.Application.DTOs;

namespace Mostruario.Application.Interfaces.Brands;

public interface IGetAllBrandsUseCase
{
    Task<PagedResult<BrandDto>> ExecuteAsync(
        int page = 1,
        int pageSize = 10,
        bool? isActive = null);
}

public interface IGetBrandByIdUseCase
{
    Task<BrandDto> ExecuteAsync(Guid id);
}

public interface ICreateBrandUseCase
{
    Task<BrandDto> ExecuteAsync(CreateBrandDto dto);
}

public interface IUpdateBrandUseCase
{
    Task<BrandDto> ExecuteAsync(UpdateBrandDto dto);
}

public interface IDeleteBrandUseCase
{
    Task ExecuteAsync(Guid id);
}
```

### Explicação

- **cada interface**: Representa uma operação específica
- **nomenclatura**: Verbo + Recurso + UseCase
- **retorno**: Task com DTO apropriado ou Task vazio para Delete

---

## Etapa 6: Implementar os Use Cases

### Objetivo

Implementar a lógica de negócio para cada operação.

### Localização

`src/Mostruario.Application/UseCases/Brands/`

### 6.1 CreateBrandUseCase.cs

```csharp
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Brands;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Brands;

public class CreateBrandUseCase(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork)
    : ICreateBrandUseCase
{
    private readonly IBrandRepository _brandRepository = brandRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<BrandDto> ExecuteAsync(CreateBrandDto dto)
    {
        var brand = BrandMapper.ToEntity(dto);
        
        await _brandRepository.AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();

        return BrandMapper.ToDto(brand);
    }
}
```

### 6.2 GetAllBrandsUseCase.cs

```csharp
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Brands;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Brands;

public class GetAllBrandsUseCase(
    IBrandRepository brandRepository)
    : IGetAllBrandsUseCase
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<PagedResult<BrandDto>> ExecuteAsync(int page, int pageSize, bool? isActive)
    {
        var (items, totalCount) = await _brandRepository.GetAllAsync(page, pageSize, isActive);
        
        var dtos = items.Select(BrandMapper.ToDto).ToList();
        
        return new PagedResult<BrandDto>(dtos, totalCount, page, pageSize);
    }
}
```

### 6.3 GetBrandByIdUseCase.cs

```csharp
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Brands;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Brands;

public class GetBrandByIdUseCase(
    IBrandRepository brandRepository)
    : IGetBrandByIdUseCase
{
    private readonly IBrandRepository _brandRepository = brandRepository;

    public async Task<BrandDto> ExecuteAsync(Guid id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        
        if (brand == null)
            throw new NotFoundException("Brand", id);

        return BrandMapper.ToDto(brand);
    }
}
```

### 6.4 UpdateBrandUseCase.cs

```csharp
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Brands;
using Mostruario.Application.Mappings;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Brands;

public class UpdateBrandUseCase(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork)
    : IUpdateBrandUseCase
{
    private readonly IBrandRepository _brandRepository = brandRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<BrandDto> ExecuteAsync(UpdateBrandDto dto)
    {
        var brand = await _brandRepository.GetByIdAsync(dto.Id);
        
        if (brand == null)
            throw new NotFoundException("Brand", dto.Id);

        BrandMapper.UpdateEntity(brand, dto);
        
        await _brandRepository.UpdateAsync(brand);
        await _unitOfWork.SaveChangesAsync();

        return BrandMapper.ToDto(brand);
    }
}
```

### 6.5 DeleteBrandUseCase.cs

```csharp
using Mostruario.Application.Interfaces.Brands;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;

namespace Mostruario.Application.UseCases.Brands;

public class DeleteBrandUseCase(
    IBrandRepository brandRepository,
    IUnitOfWork unitOfWork)
    : IDeleteBrandUseCase
{
    private readonly IBrandRepository _brandRepository = brandRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task ExecuteAsync(Guid id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        
        if (brand == null)
            throw new NotFoundException("Brand", id);

        await _brandRepository.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
```

### Explicação

- **padrão de construtor**: Use cases recebem dependências via injeção de construtor
- **validação**: NotFoundException se entidade não existe
- **transações**: IUnitOfWork.SaveChangesAsync() para persistir alterações

---

## Etapa 7: Criar o Controller

### Objetivo

Expor os use cases como endpoints HTTP.

### Localização

`src/Mostruario.Api/Controllers/BrandsController.cs`

### Implementação

```csharp
using Microsoft.AspNetCore.Mvc;
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Brands;
using Microsoft.AspNetCore.Authorization;

namespace Mostruario.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BrandsController(
    IGetAllBrandsUseCase getAllUseCase,
    IGetBrandByIdUseCase getByIdUseCase,
    ICreateBrandUseCase createUseCase,
    IUpdateBrandUseCase updateUseCase,
    IDeleteBrandUseCase deleteUseCase)
    : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<BrandDto>>> GetAll(
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
    public async Task<ActionResult<BrandDto>> GetById(Guid id)
    {
        var result = await getByIdUseCase.ExecuteAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<BrandDto>> Create([FromBody] CreateBrandDto dto)
    {
        var result = await createUseCase.ExecuteAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<BrandDto>> Update(Guid id, [FromBody] UpdateBrandDto dto)
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

### Explicação

- **[ApiController]**: Ativa recursos de API (model binding, validação automática)
- **[Route("api/v1/[controller]")]**: URL base /api/v1/brands
- **[AllowAnonymous]**: Endpoints GET públicos
- **[Authorize]**: Endpoints que requerem autenticação
- **CreatedAtAction**: Retorna 201 Created com Location header

---

## Etapa 8: Registrar as Dependências

### Objetivo

Configurar injeção de dependência no Program.cs.

### Localização

`src/Mostruario.Api/Extensions/DependencyInjection.cs`

### Implementação

Adicione ao método de extensão existente:

```csharp
// Brands
services.AddScoped<IGetAllBrandsUseCase, GetAllBrandsUseCase>();
services.AddScoped<IGetBrandByIdUseCase, GetBrandByIdUseCase>();
services.AddScoped<ICreateBrandUseCase, CreateBrandUseCase>();
services.AddScoped<IUpdateBrandUseCase, UpdateBrandUseCase>();
services.AddScoped<IDeleteBrandUseCase, DeleteBrandUseCase>();
```

### Explicação

- **AddScoped**: Instância criada por requisição HTTP
- **interface → implementação**: Permite mocking em testes

---

## Etapa 9: Criar os Testes Unitários

### Objetivo

Garantir funcionamento correto dos Use Cases.

### Localização

`tests/Mostruario.Application.Tests/UseCases/CreateBrandUseCaseTests.cs`

### Implementação

```csharp
using FluentAssertions;
using Mostruario.Application.DTOs;
using Mostruario.Application.UseCases.Brands;
using Mostruario.Domain.Entities;
using Mostruario.Domain.Exceptions;
using Mostruario.Domain.Interfaces;
using Moq;
using Xunit;

namespace Mostruario.Application.Tests.UseCases;

public class CreateBrandUseCaseTests
{
    private readonly Mock<IBrandRepository> _brandRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateBrandUseCase _useCase;

    public CreateBrandUseCaseTests()
    {
        _brandRepositoryMock = new Mock<IBrandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _useCase = new CreateBrandUseCase(
            _brandRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_ReturnsBrandDto()
    {
        // Arrange
        var dto = new CreateBrandDto
        {
            Name = "Nike",
            Description = "Marca esportiva"
        };

        Brand? savedBrand = null;
        _brandRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Brand>()))
            .Callback<Brand>(b => savedBrand = b)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.ExecuteAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(dto.Name);
        result.Description.Should().Be(dto.Description);
        result.IsActive.Should().BeTrue();

        _brandRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Brand>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyName_ThrowsBusinessRuleException()
    {
        // Arrange
        var dto = new CreateBrandDto
        {
            Name = "",
            Description = "Description"
        };

        // Act & Assert
        var act = () => _useCase.ExecuteAsync(dto);
        await act.Should().ThrowAsync<BusinessRuleException>()
            .WithMessage("*required*");
    }
}
```

### Explicação

- **FluentAssertions**: API fluente para assertions
- **Moq**: Framework para criação de mocks
- **Xunit**: Framework de testes com [Fact]
- **Arrange-Act-Assert**: Padrão AAA

---

## Endpoints Resultantes

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | /api/v1/brands | Listar todas as marcas (paginado) |
| GET | /api/v1/brands/{id} | Obter marca por ID |
| POST | /api/v1/brands | Criar nova marca |
| PUT | /api/v1/brands/{id} | Atualizar marca |
| DELETE | /api/v1/brands/{id} | Excluir marca |

---

## Checklist Resumo

- [ ] Criar entidade em Domain/Entities/Brand.cs
- [ ] Criar interface IBrandRepository em Domain/Interfaces/
- [ ] Criar DTOs em Application/DTOs/
- [ ] Criar mapper em Application/Mappings/
- [ ] Criar interfaces de Use Cases em Application/Interfaces/Brands/
- [ ] Implementar Use Cases em Application/UseCases/Brands/
- [ ] Criar Controller em Api/Controllers/
- [ ] Registrar dependências em Extensions/DependencyInjection.cs
- [ ] Criar testes em tests/UseCases/