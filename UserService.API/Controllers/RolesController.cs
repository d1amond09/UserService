using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Common.DTOs;
using UserService.Application.UseCases.Users.AddUserToRole;
using UserService.Application.UseCases.Users.RemoveUserFromRole;
using UserService.Domain.Common.Constants;

namespace UserService.API.Controllers
{
	[Route("api/users/{id:guid}/roles")]
	[ApiController]
	public class RolesController(ISender sender) : ControllerBase
	{
		private readonly ISender _sender = sender;
		
		[Authorize(Roles = Roles.Admin)]
		[HttpPost]
		public async Task<IActionResult> AddUserToRole(Guid id, [FromBody] RoleDto request)
		{
			await _sender.Send(new AddUserToRoleCommand(id, request.RoleName));
			return NoContent();
		}


		[Authorize(Roles = Roles.Admin)]
		[HttpDelete("{roleName}")]
		public async Task<IActionResult> RemoveRoleFromUser(Guid id, string roleName)
		{
			await _sender.Send(new RemoveUserFromRoleCommand(id, roleName));
			return NoContent();
		}
	}
}
