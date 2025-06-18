using System.ComponentModel.DataAnnotations;

namespace UserService.API.Contracts.Requests;

public record AddUserToRoleRequest(string RoleName);
