using MediatR;
using UserService.Application.Common.Enums;

namespace UserService.Application.UseCases.Users.UpdateUserStatus;

public record UpdateUserStatusCommand(Guid UserId, UserStatusAction Action) : IRequest;