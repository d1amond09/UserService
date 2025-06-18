using MediatR;

namespace UserService.Application.UseCases.Users.AddUserToRole;

public sealed record AddUserToRoleCommand(Guid UserId, string RoleName) : IRequest;