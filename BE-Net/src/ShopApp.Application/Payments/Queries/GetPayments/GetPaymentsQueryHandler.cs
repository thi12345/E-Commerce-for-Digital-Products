using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common;
using ShopApp.Application.Payments.DTOs;
using ShopApp.Domain.Payments.Entities;
using ShopApp.Domain.Payments.Repositories;

namespace ShopApp.Application.Payments.Queries.GetPayments;

public sealed class GetPaymentsQueryHandler(
    IPaymentRepository paymentRepository,
    ILogger<GetPaymentsQueryHandler> logger)
    : IRequestHandler<GetPaymentsQuery, PagedResult<PaymentDto>>
{
    public async Task<PagedResult<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken ct)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var page = Math.Max(request.Page, 1);

        logger.LogDebug("Fetching payments page={Page} pageSize={PageSize}", page, pageSize);

        var (items, totalCount) = await paymentRepository.GetPagedAsync(
            request.UserId, request.OrderId, request.Status, page, pageSize, ct);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedResult<PaymentDto>(
            items.Select(ToDto).ToList().AsReadOnly(),
            totalCount, totalPages, page, pageSize);
    }

    private static PaymentDto ToDto(Payment p) =>
        new(p.Id, p.OrderId, p.UserId, p.Amount.Amount, p.Amount.Currency,
            p.Method.ToString(), p.Status.ToString(), p.TransactionId, p.PaidAt, p.CreatedAt);
}
