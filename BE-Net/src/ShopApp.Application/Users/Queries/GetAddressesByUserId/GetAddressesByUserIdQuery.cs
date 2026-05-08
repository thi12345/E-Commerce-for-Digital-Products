using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Queries.GetAddressesByUserId;

public record GetAddressesByUserIdQuery(Guid UserId) : IRequest<IReadOnlyList<AddressDto>>;
