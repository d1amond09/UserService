using UserService.Application.Common.Responses;
using UserService.Application.Common.DTOs;
using MediatR;

namespace UserService.Application.UseCases.Auth.LoginUser;

public sealed record LoginUserCommand(UserForLoginDto UserToLogin) :
	IRequest<TokenDto>;