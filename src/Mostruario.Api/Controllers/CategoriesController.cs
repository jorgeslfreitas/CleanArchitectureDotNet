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