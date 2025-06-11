using UserService.Application.Common.DTOs;
using UserService.Domain.Common.RequestFeatures;
using MediatR;
using UserService.Application.Common.Responses;
using AutoMapper;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace UserService.Application.UseCases.Users.GetUserMe;

public class GetUserMeHandler(IMapper mapper, ICurrentUserService currentUserService, UserManager<User> userManager) 
	: IRequestHandler<GetUserMeQuery, ApiBaseResponse>
{
	private readonly IMapper _mapper = mapper;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly UserManager<User> _userManager = userManager; 

	public async Task<ApiBaseResponse> Handle(GetUserMeQuery request, CancellationToken cancellationToken)
	{
		var user = await _currentUserService.GetCurrentUserAsync(cancellationToken);
		if (user == null)
		{
			return new ApiNotFoundResponse("Not found user");
		}

		var userDto = _mapper.Map<UserDetailsDto>(user);
		userDto.Roles = await _userManager.GetRolesAsync(user); 

		return new ApiOkResponse<UserDetailsDto>(userDto);
	}
}