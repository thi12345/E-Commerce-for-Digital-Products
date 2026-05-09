using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApp.API.Auth;
using ShopApp.Application.Payments.Commands.CreatePayment;
using ShopApp.Application.Payments.Commands.DeletePayment;
using ShopApp.Application.Payments.Commands.UpdatePaymentStatus;
using ShopApp.Application.Payments.Queries.GetPaymentById;
using ShopApp.Application.Payments.Queries.GetPayments;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PaymentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? orderId,
        [FromQuery] PaymentStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetPaymentsQuery(userId, orderId, status, page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentByIdQuery(id), ct);
        if (result is not null && !User.CanAccessUser(result.UserId))
            return Forbid();

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.CustomerOrAdmin)]
    public async Task<IActionResult> Create([FromBody] CreatePaymentCommand command, CancellationToken ct)
    {
        if (!User.CanAccessUser(command.UserId))
            return Forbid();

        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdatePaymentStatusCommand(id, request.NewStatus, request.TransactionId), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdminOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeletePaymentCommand(id), ct);
        return NoContent();
    }
}

public record UpdatePaymentStatusRequest(
    PaymentStatus NewStatus,
    string? TransactionId = null);
