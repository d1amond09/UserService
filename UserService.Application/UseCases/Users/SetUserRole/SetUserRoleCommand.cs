using UserService.Application.Common.Responses;
using MediatR;

namespace UserService.Application.UseCases.Users.SetUserRole;

public sealed record SetUserRoleCommand(Guid UserId, string RoleName, bool AddRole) 
	: IRequest<ApiBaseResponse>;