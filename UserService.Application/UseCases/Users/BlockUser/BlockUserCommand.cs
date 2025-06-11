using UserService.Application.Common.Responses;
using MediatR;

namespace UserService.Application.UseCases.Users.BlockUser;

public sealed record BlockUserCommand(Guid UserId) : IRequest<ApiBaseResponse>;
