using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.SetDefaultAddress;

public sealed class SetDefaultAddressCommandHandler(
    IAddressRepository addressRepository,
    IUnitOfWork unitOfWork,
    ILogger<SetDefaultAddressCommandHandler> logger)
    : IRequestHandler<SetDefaultAddressCommand, AddressDto>
{
    public async Task<AddressDto> Handle(SetDefaultAddressCommand request, CancellationToken ct)
    {
        var allAddresses = await addressRepository.GetByUserIdAsync(request.UserId, ct);

        var target = allAddresses.FirstOrDefault(a => a.Id == request.AddressId)
            ?? throw new DomainException($"Address '{request.AddressId}' not found for user '{request.UserId}'.");

        foreach (var a in allAddresses.Where(a => a.IsDefault))
        {
            a.ClearDefault();
            addressRepository.Update(a);
        }

        target.SetAsDefault();
        addressRepository.Update(target);

        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Default address set: AddressId={AddressId}, UserId={UserId}",
            request.AddressId, request.UserId);

        return ToDto(target);
    }

    private static AddressDto ToDto(Address a) =>
        new(a.Id, a.UserId, a.FullName, a.PhoneNumber, a.AddressLine,
            a.City, a.Province, a.Country, a.PostalCode, a.IsDefault, a.CreatedAt);
}
