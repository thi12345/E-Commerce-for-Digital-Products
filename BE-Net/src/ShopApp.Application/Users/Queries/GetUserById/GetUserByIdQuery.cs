using MediatR;
using ShopApp.Application.Users.DTOs;

namespace ShopApp.Application.Users.Queries.GetUserById;

public record GetUserByIdQuery(Guid Id) : IRequest<UserDto?>;
