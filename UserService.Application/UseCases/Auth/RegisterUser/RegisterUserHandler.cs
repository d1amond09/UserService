using UserService.Application.Common.Responses;
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
	private readonly IEmailService _emailService = emailService;
	private readonly UserManager<User> _userManager = userManager;
	private readonly IMapper _mapper = mapper;

	public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
	{
		var email = request.UserForRegistrationDto.Email;

		User user = _mapper.Map<User>(request.UserForRegistrationDto);

		string password = request.UserForRegistrationDto.Password 
			?? throw new BadRequestException("Password is null");
		
		var result = await _userManager.CreateAsync(user, password);

		if (!result.Succeeded)
		{
			var errors = result.Errors
				.ToDictionary(e => e.Code, e => new[] { e.Description });

			throw new ValidationException(errors);
		}

		await _userManager.AddToRoleAsync(user, "User");

		var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		
		await _emailService.SendEmailConfirmationAsync(user, token);

		return user.Id;
	}
}