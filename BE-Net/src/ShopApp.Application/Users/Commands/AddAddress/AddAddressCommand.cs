using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Commands.AddAddress;

public record AddAddressCommand(
    Guid UserId,
    string FullName,
    string PhoneNumber,
    string AddressLine,
    string City,
    string Province,
    string Country,
    string? PostalCode = null) : IRequest<AddressDto>;
