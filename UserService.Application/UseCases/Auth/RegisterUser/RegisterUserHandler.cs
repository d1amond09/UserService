using Microsoft.AspNetCore.Identity;
using UserService.Domain.Users;
using AutoMapper;
using MediatR;
using UserService.Domain.Common.Constants;
using UserService.Application.Common.Interfaces.Persistence;
using System.Diagnostics;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces;

namespace UserService.Application.UseCases.Auth.RegisterUser;

public class RegisterUserHandler(UserManager<User> userManager, IMapper mapper, IEmailService emailService) 
	: IRequestHandler<RegisterUserCommand, Guid>
{
	public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
	{
		var email = request.UserForRegistrationDto.Email;

		User user = mapper.Map<User>(request.UserForRegistrationDto);

		string password = request.UserForRegistrationDto.Password 
			?? throw new BadRequestException("Password is null");
		
		var result = await userManager.CreateAsync(user, password);

		if (!result.Succeeded)
		{
			var errors = result.Errors
				.ToDictionary(e => e.Code, e => new[] { e.Description });

			throw new ValidationException(errors);
		}

		await userManager.AddToRoleAsync(user, Roles.User);

		var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
		
		await emailService.SendEmailConfirmationAsync(user, token);

		return user.Id;
	}
}