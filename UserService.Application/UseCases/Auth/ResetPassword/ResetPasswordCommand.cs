using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace UserService.Application.UseCases.Auth.ResetPassword;

public record ResetPasswordCommand(string Email, string Password, string ConfirmPassword, string Token) : IRequest;
