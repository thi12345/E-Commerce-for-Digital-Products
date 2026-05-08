using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopApp.Application.Catalog.Commands.CreateReview;
using ShopApp.Application.Catalog.Commands.DeleteReview;
using ShopApp.Application.Catalog.Commands.UpdateReview;
using ShopApp.Application.Catalog.Queries.GetReviewById;
using ShopApp.Application.Catalog.Queries.GetReviews;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/products/{productId:guid}/reviews")]
public sealed class ReviewsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken ct)
    {
        var result = await sender.Send(new GetReviewsQuery(productId), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid productId, Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetReviewByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Guid productId, [FromBody] CreateReviewRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new CreateReviewCommand(productId, request.UserId, request.Rating, request.ReviewTitle, request.ReviewContent), ct);
        return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid productId, Guid id, [FromBody] UpdateReviewRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateReviewCommand(id, request.Rating, request.ReviewTitle, request.ReviewContent), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid productId, Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteReviewCommand(id), ct);
        return NoContent();
    }
}

public record CreateReviewRequest(Guid UserId, decimal Rating, string ReviewTitle, string ReviewContent);
public record UpdateReviewRequest(decimal Rating, string ReviewTitle, string ReviewContent);
