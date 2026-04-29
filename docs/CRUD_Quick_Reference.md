# CRUD Quick Reference

Guia rápido para criação de CRUD. Para detalhes completos, consulte `CRUD_Step_by_Step_Guide.md`.

---

## Estrutura de Arquivos

```
src/Mostruario.Domain/
  Entities/               → Entidade.cs
  Interfaces/             → IEntidadeRepository.cs

src/Mostruario.Application/
  DTOs/                   → Dto.cs, CreateDto.cs, UpdateDto.cs
  Mappings/               → EntidadeMapper.cs
  Interfaces/Entidade/    → IEntidadeUseCases.cs
  UseCases/Entidade/      → *EntidadeUseCase.cs (5 arquivos)

src/Mostruario.Api/
  Controllers/            → EntidadeController.cs

tests/
  UseCases/               → *EntidadeUseCaseTests.cs
```

---

## Checklist de Criação

### Domain Layer

- [ ] **Entidade**: Herdar `BaseEntity<Guid>`, adicionar propriedades, construtor com validação, métodos de domínio
- [ ] **Interface Repositório**: Herdar `IRepository<T>`, adicionar métodos específicos

### Application Layer

- [ ] **DTOs**: `EntidadeDto`, `CreateEntidadeDto`, `UpdateEntidadeDto`
- [ ] **Mapper**: `ToDto()`, `ToEntity()`, `UpdateEntity()`
- [ ] **Interfaces Use Cases**: 5 interfaces (GetAll, GetById, Create, Update, Delete)
- [ ] **Implementação Use Cases**:
  - Create: Instanciar → AddAsync → SaveChangesAsync → Retornar DTO
  - GetAll: GetAllAsync → Mapear → Retornar PagedResult
  - GetById: GetByIdAsync → NotFoundException? → Retornar DTO
  - Update: GetByIdAsync → NotFoundException? → UpdateEntity → UpdateAsync → SaveChangesAsync
  - Delete: GetByIdAsync → NotFoundException? → DeleteAsync → SaveChangesAsync

### API Layer

- [ ] **Controller**: Injetar 5 Use Cases → Endpoints HTTP com atributos [AllowAnonymous]/[Authorize]

---

## Padrões de Código

### Atributos de Rotas

```csharp
[ApiController]
[Route("api/v1/[controller]")]
public class EntidadesController(...) : ControllerBase
```

### Endpoints

| Verbo | Rota | Atributo | Retorno |
|-------|------|----------|---------|
| GET | / | [AllowAnonymous] | PagedResult |
| GET | /{id:guid} | [AllowAnonymous] | DTO |
| POST | / | [Authorize] | 201 Created |
| PUT | /{id:guid} | [Authorize] | DTO |
| DELETE | /{id:guid} | [Authorize] | 204 NoContent |

### Validação

```csharp
// No controller
if (page < 1 || pageSize < 1 || pageSize > 100)
    return BadRequest("Invalid pagination parameters");

// No use case
if (entity == null)
    throw new NotFoundException("Entity", id);
```

---

## Injeção de Dependência

Adicionar em `src/Mostruario.Api/Extensions/DependencyInjection.cs`:

```csharp
// Entidade
services.AddScoped<IGetAllEntidadesUseCase, GetAllEntidadesUseCase>();
services.AddScoped<IGetEntidadeByIdUseCase, GetEntidadeByIdUseCase>();
services.AddScoped<ICreateEntidadeUseCase, CreateEntidadeUseCase>();
services.AddScoped<IUpdateEntidadeUseCase, UpdateEntidadeUseCase>();
services.AddScoped<IDeleteEntidadeUseCase, DeleteEntidadeUseCase>();
```

---

## Testes Unitários

### Estrutura Base

```csharp
public class CreateEntidadeUseCaseTests
{
    private readonly Mock<IEntidadeRepository> _repoMock;
    private readonly Mock<IUnitOfWork> _unitMock;
    private readonly CreateEntidadeUseCase _useCase;

    public CreateEntidadeUseCaseTests()
    {
        _repoMock = new Mock<IEntidadeRepository>();
        _unitMock = new Mock<IUnitOfWork>();
        _useCase = new CreateEntidadeUseCase(
            _repoMock.Object,
            _unitMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidDto_ReturnsDto()
    {
        // Arrange
        var dto = new CreateEntidadeDto { /* campos */ };
        
        // Act
        var result = await _useCase.ExecuteAsync(dto);
        
        // Assert
        result.Should().NotBeNull();
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Entidade>()), Times.Once);
    }
}
```

### Executar Testes

```bash
dotnet test
```

---

## Comandos Úteis

```bash
# Criar entidade completa (Manual)
# Seguir estrutura existente em src/Mostruario.Domain/Entities/

# Criar use case
# Copiar estrutura de CreateCategoryUseCase.cs e adaptar

# Verificar compilação
dotnet build
```
