using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.AddAddress;

public sealed class AddAddressCommandHandler(
    IUserRepository userRepository,
    IAddressRepository addressRepository,
    IUnitOfWork unitOfWork,
    ILogger<AddAddressCommandHandler> logger)
    : IRequestHandler<AddAddressCommand, AddressDto>
{
    public async Task<AddressDto> Handle(AddAddressCommand request, CancellationToken ct)
    {
        var userExists = await userRepository.GetByIdAsync(request.UserId, ct);
        if (userExists is null)
            throw new DomainException($"User '{request.UserId}' not found.");

        var address = Address.Create(
            request.UserId,
            request.FullName,
            request.PhoneNumber,
            request.AddressLine,
            request.City,
            request.Province,
            request.Country,
            request.PostalCode);

        // Tự động set default nếu đây là địa chỉ đầu tiên của user
        var existing = await addressRepository.GetByUserIdAsync(request.UserId, ct);
        if (!existing.Any())
            address.SetAsDefault();

        await addressRepository.AddAsync(address, ct);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Address added: Id={AddressId}, UserId={UserId}", address.Id, address.UserId);

        return ToDto(address);
    }

    private static AddressDto ToDto(Address a) =>
        new(a.Id, a.UserId, a.FullName, a.PhoneNumber, a.AddressLine,
            a.City, a.Province, a.Country, a.PostalCode, a.IsDefault, a.CreatedAt);
}
