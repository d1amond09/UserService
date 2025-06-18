using MediatR;

namespace UserService.Application.UseCases.Auth.ConfirmEmail;

public record ConfirmEmailCommand(Guid UserId, string Token) : IRequest;
