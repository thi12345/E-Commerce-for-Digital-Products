using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Commands.SetDefaultAddress;

public record SetDefaultAddressCommand(Guid UserId, Guid AddressId) : IRequest<AddressDto>;
