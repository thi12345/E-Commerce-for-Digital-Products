using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Catalog.Commands.CreateCategory;
using ShopApp.Application.Catalog.Commands.DeleteCategory;
using ShopApp.Application.Catalog.Commands.UpdateCategory;
using ShopApp.Application.Catalog.Queries.GetCategories;
using ShopApp.Application.Catalog.Queries.GetCategoryById;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoriesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetCategoryByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateCategoryCommand(id, request.Name, request.Description), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteCategoryCommand(id), ct);
        return NoContent();
    }
}

public record UpdateCategoryRequest(string Name, string? Description);
