using MediatR;
using Microsoft.AspNetCore.Http;

namespace UserService.Application.UseCases.Users.UploadAvatar;

public record UploadAvatarCommand(IFormFile File) : IRequest<string>;
