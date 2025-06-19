using MediatR;

namespace UserService.Application.UseCases.Auth.ForgotPassword;

public record ForgotPasswordCommand(string Email) : IRequest;
