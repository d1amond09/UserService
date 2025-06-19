using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Users.UpdateUserByAdmin;

public record UpdateUserByAdminCommand(Guid UserId, UpdateUserDto Dto) : IRequest;
