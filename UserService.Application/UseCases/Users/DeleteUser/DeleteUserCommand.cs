using MediatR;

namespace UserService.Application.UseCases.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest;
