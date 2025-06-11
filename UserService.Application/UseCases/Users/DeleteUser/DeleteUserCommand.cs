using UserService.Application.Common.Responses;
using MediatR;

namespace UserService.Application.UseCases.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<ApiBaseResponse>;
