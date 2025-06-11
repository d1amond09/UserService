using UserService.Application.Common.Responses;
using MediatR;

namespace UserService.Application.UseCases.Users.GetUserMe;

public sealed record GetUserMeQuery() : IRequest<ApiBaseResponse>;
