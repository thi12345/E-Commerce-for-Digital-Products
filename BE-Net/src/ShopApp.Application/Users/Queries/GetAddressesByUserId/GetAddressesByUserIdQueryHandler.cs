using MediatR;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Queries.GetAddressesByUserId;

public sealed class GetAddressesByUserIdQueryHandler(IAddressRepository addressRepository)
    : IRequestHandler<GetAddressesByUserIdQuery, IReadOnlyList<AddressDto>>
{
    public async Task<IReadOnlyList<AddressDto>> Handle(GetAddressesByUserIdQuery request, CancellationToken ct)
    {
        var addresses = await addressRepository.GetByUserIdAsync(request.UserId, ct);
        return addresses.Select(ToDto).ToList().AsReadOnly();
    }

    private static AddressDto ToDto(Address a) =>
        new(a.Id, a.UserId, a.FullName, a.PhoneNumber, a.AddressLine,
            a.City, a.Province, a.Country, a.PostalCode, a.IsDefault, a.CreatedAt);
}
