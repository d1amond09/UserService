using UserService.Application.Common.Responses;
using UserService.Domain.Users;
using MediatR;

namespace UserService.Application.UseCases.Auth.CreateToken;

public sealed record CreateTokenCommand(User User, bool PopulateExp) :
	IRequest<ApiBaseResponse>;