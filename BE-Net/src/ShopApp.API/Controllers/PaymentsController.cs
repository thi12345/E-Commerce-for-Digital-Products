using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePaymentStatusRequest request, CancellationToken ct)
    {
        var result = await sender.Send(
            new UpdatePaymentStatusCommand(id, request.NewStatus, request.TransactionId), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeletePaymentCommand(id), ct);
        return NoContent();
    }
}

public record UpdatePaymentStatusRequest(
    PaymentStatus NewStatus,
    string? TransactionId = null);
