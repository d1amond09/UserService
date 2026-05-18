using MediatR;

namespace UserService.Application.UseCases.Auth.ResetPassword;

public record ResetPasswordCommand(string Email, string Password, string ConfirmPassword, string Token) : IRequest;
