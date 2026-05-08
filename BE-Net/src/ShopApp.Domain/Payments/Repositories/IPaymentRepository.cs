using ShopApp.Domain.Payments.Entities;
using ShopApp.Domain.Payments.Enums;

namespace ShopApp.Domain.Payments.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Payment> Items, int TotalCount)> GetPagedAsync(
        Guid? userId, Guid? orderId, PaymentStatus? status,
        int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(Payment payment, CancellationToken ct = default);
    void Update(Payment payment);
    void Delete(Payment payment);
}
