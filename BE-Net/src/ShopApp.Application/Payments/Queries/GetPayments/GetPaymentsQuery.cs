using MediatR;
using ShopApp.Application.Common;
using ShopApp.Application.Payments.DTOs;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.Application.Payments.Queries.GetPayments;

public record GetPaymentsQuery(
    Guid? UserId = null,
    Guid? OrderId = null,
    PaymentStatus? Status = null,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<PaymentDto>>;
