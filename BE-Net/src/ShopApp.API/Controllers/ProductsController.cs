using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.API.Auth;
using ShopApp.Application.Catalog.Commands.CreateProduct;
using ShopApp.Application.Catalog.Commands.DeleteProduct;
using ShopApp.Application.Catalog.Commands.UpdateProduct;
using ShopApp.Application.Catalog.Queries.GetProductById;
using ShopApp.Application.Catalog.Queries.GetProducts;
using ShopApp.Domain.Catalog.Enums;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] Guid? categoryId,
        [FromQuery] decimal? minRating,
        [FromQuery] decimal? minDiscountPercentage,
        [FromQuery] ProductSortBy sortBy = ProductSortBy.Default,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new GetProductsQuery(name, categoryId, minRating, minDiscountPercentage, sortBy, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetProductByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateProductCommand(
            id,
            request.Name,
            request.AboutProduct,
            request.ImgLink,
            request.Rating,
            request.RatingCount,
            request.CategoryId,
            request.Variants), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}

public record UpdateProductRequest(
    string Name,
    string AboutProduct,
    string ImgLink,
    decimal Rating = 0,
    int RatingCount = 0,
    Guid? CategoryId = null,
    IReadOnlyList<ProductVariantRequest>? Variants = null);
