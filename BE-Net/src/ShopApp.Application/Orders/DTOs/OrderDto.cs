namespace ShopApp.Application.Orders.DTOs;

public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    decimal TotalAmount,
    string Currency,
    IReadOnlyList<OrderItemDto> Items,
    DateTime CreatedAt);

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal TotalPrice);
