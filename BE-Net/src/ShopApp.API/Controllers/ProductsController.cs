using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Catalog.Commands.ActivateProduct;
using ShopApp.Application.Catalog.Commands.CreateProduct;
using ShopApp.Application.Catalog.Commands.DeactivateProduct;
using ShopApp.Application.Catalog.Commands.DeleteProduct;
using ShopApp.Application.Catalog.Commands.UpdateProduct;
using ShopApp.Application.Catalog.Queries.GetProductById;
using ShopApp.Application.Catalog.Queries.GetProducts;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetProductsQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateProductCommand(
            id, request.Name, request.Description, request.Price, request.Currency, request.DownloadUrl), ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new ActivateProductCommand(id), ct));

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new DeactivateProductCommand(id), ct));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}

public record UpdateProductRequest(
    string Name, string Description, decimal Price, string Currency, string DownloadUrl);
