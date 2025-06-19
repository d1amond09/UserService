using UserService.Application.Common.DTOs;
using MediatR;

namespace UserService.Application.UseCases.Auth.RegisterUser;

public sealed record RegisterUserCommand(UserForRegistrationDto UserForRegistrationDto) :
	IRequest<Guid>;