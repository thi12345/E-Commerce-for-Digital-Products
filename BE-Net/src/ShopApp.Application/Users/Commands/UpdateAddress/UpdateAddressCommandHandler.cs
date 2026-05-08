using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Application.Users.DTOs;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Entities;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.UpdateAddress;

public sealed class UpdateAddressCommandHandler(
    IAddressRepository addressRepository,
    IUnitOfWork unitOfWork,
    ILogger<UpdateAddressCommandHandler> logger)
    : IRequestHandler<UpdateAddressCommand, AddressDto>
{
    public async Task<AddressDto> Handle(UpdateAddressCommand request, CancellationToken ct)
    {
        var address = await addressRepository.GetByIdAsync(request.AddressId, ct)
            ?? throw new DomainException($"Address '{request.AddressId}' not found.");

        address.Update(
            request.FullName,
            request.PhoneNumber,
            request.AddressLine,
            request.City,
            request.Province,
            request.Country,
            request.PostalCode);

        addressRepository.Update(address);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Address updated: Id={AddressId}", address.Id);

        return ToDto(address);
    }

    private static AddressDto ToDto(Address a) =>
        new(a.Id, a.UserId, a.FullName, a.PhoneNumber, a.AddressLine,
            a.City, a.Province, a.Country, a.PostalCode, a.IsDefault, a.CreatedAt);
}
