using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.API.Auth;
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
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> Create(Guid productId, [FromBody] CreateReviewRequest request, CancellationToken ct)
    {
        if (!User.CanAccessUser(request.UserId))
            return Forbid();

        var result = await sender.Send(
            new CreateReviewCommand(productId, request.UserId, request.Rating, request.ReviewTitle, request.ReviewContent), ct);
        return CreatedAtAction(nameof(GetById), new { productId, id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> Update(Guid productId, Guid id, [FromBody] UpdateReviewRequest request, CancellationToken ct)
    {
        var review = await sender.Send(new GetReviewByIdQuery(id), ct);
        if (review is null)
            return NotFound();
        if (!User.CanAccessUser(review.UserId))
            return Forbid();

        var result = await sender.Send(new UpdateReviewCommand(id, request.Rating, request.ReviewTitle, request.ReviewContent), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> Delete(Guid productId, Guid id, CancellationToken ct)
    {
        var review = await sender.Send(new GetReviewByIdQuery(id), ct);
        if (review is null)
            return NotFound();
        if (!User.CanAccessUser(review.UserId))
            return Forbid();

        await sender.Send(new DeleteReviewCommand(id), ct);
        return NoContent();
    }
}

public record CreateReviewRequest(Guid UserId, decimal Rating, string ReviewTitle, string ReviewContent);
public record UpdateReviewRequest(decimal Rating, string ReviewTitle, string ReviewContent);
