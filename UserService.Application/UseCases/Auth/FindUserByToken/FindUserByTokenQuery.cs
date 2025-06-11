using UserService.Application.Common.Responses;
using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Auth.FindUserByToken;

public sealed record FindUserByTokenQuery(TokenDto TokenDto) :
	IRequest<ApiBaseResponse>;