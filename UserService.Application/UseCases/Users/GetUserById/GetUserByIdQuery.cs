using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Users.GetUserById;

public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserDetailsDto>;
