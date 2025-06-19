using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Users.UpdateUser;

public record UpdateUserCommand(UpdateUserDto Dto) : IRequest;
