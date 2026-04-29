using Microsoft.AspNetCore.Mvc;
using Mostruario.Application.DTOs;
using Mostruario.Application.Interfaces.Products;
using Microsoft.AspNetCore.Authorization;

namespace Mostruario.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController(
    IGetAllProductsUseCase getAllUseCase,
    IGetProductByIdUseCase getByIdUseCase,
    ICreateProductUseCase createUseCase,
    IUpdateProductUseCase updateUseCase,
    IDeleteProductUseCase deleteUseCase) : ControllerBase
{

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isActive = null)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
            return BadRequest("Invalid pagination parameters");

        var result = await getAllUseCase.ExecuteAsync(page, pageSize, search, categoryId, isActive);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var result = await getByIdUseCase.ExecuteAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var result = await createUseCase.ExecuteAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto)
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