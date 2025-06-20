using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Auth.ExternalLogin;
public record ExternalLoginCommand(
	string IdToken) : IRequest<TokenDto>;
