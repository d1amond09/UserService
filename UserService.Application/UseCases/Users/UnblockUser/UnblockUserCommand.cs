using UserService.Application.Common.Responses;
using MediatR;

namespace UserService.Application.UseCases.Users.UnblockUser;

public sealed record UnblockUserCommand(Guid UserId) : IRequest<ApiBaseResponse>;
