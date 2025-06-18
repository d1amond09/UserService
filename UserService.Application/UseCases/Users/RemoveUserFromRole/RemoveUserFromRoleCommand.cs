using MediatR;

namespace UserService.Application.UseCases.Users.RemoveUserFromRole;

public sealed record RemoveUserFromRoleCommand(Guid UserId, string RoleName) : IRequest;