using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.Common.Responses;
using MediatR;

namespace UserService.Application.UseCases.Users.GetUsers;

public sealed record GetUsersQuery(UserParameters Parameters) : IRequest<ApiBaseResponse>;
