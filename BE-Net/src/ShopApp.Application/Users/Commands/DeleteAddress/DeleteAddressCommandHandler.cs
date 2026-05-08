using MediatR;
using Microsoft.Extensions.Logging;
using ShopApp.Application.Common.Interfaces;
using ShopApp.Domain.Exceptions;
using ShopApp.Domain.Users.Repositories;

namespace ShopApp.Application.Users.Commands.DeleteAddress;

public sealed class DeleteAddressCommandHandler(
    IAddressRepository addressRepository,
    IUnitOfWork unitOfWork,
    ILogger<DeleteAddressCommandHandler> logger)
    : IRequestHandler<DeleteAddressCommand>
{
    public async Task Handle(DeleteAddressCommand request, CancellationToken ct)
    {
        var address = await addressRepository.GetByIdAsync(request.AddressId, ct)
            ?? throw new DomainException($"Address '{request.AddressId}' not found.");

        addressRepository.Delete(address);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Address deleted: Id={AddressId}", request.AddressId);
    }
}
