using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Auth.RefreshToken;

public record RefreshTokenCommand(TokenDto ExpiredTokenDto) : IRequest<TokenDto>;
