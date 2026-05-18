using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Users.IsUserBlocked;

public sealed record IsUserBlockedQuery(Guid UserId) : IRequest<IsUserBlockedDto>;
