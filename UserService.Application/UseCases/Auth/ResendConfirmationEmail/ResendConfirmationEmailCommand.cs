using MediatR;

namespace UserService.Application.UseCases.Auth.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand(string Email) : IRequest;
