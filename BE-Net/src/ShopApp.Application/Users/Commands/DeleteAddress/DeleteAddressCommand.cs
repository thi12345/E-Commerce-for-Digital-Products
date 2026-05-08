using MediatR;

namespace ShopApp.Application.Users.Commands.DeleteAddress;

public record DeleteAddressCommand(Guid AddressId) : IRequest;
