using MediatR;
using UserService.Application.Common.Attributes;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Users.GetUserById;

[Cacheable(15)]
public sealed record GetUserByIdQuery(Guid UserId) : IRequest<UserDetailsDto>;
