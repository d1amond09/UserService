using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Users.GetUserMe;

public sealed record GetUserMeQuery() : IRequest<UserDetailsDto>;
