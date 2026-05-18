using MediatR;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Auth.ForgotPassword;

public record ForgotPasswordCommand(ForgotPasswordDto forgotPassword) : IRequest;
