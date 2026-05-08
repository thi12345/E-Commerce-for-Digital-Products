using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Commands.UpdateAddress;

public record UpdateAddressCommand(
    Guid AddressId,
    string FullName,
    string PhoneNumber,
    string AddressLine,
    string City,
    string Province,
    string Country,
    string? PostalCode = null) : IRequest<AddressDto>;
