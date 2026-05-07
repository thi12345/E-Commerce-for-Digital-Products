using MediatR;

namespace ShopApp.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest;
