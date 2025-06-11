using UserService.Application.Common.DTOs;
using UserService.Domain.Common.RequestFeatures.ModelParameters;
using UserService.Domain.Common.RequestFeatures;
using MediatR;
using UserService.Application.Common.Responses;
using AutoMapper;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace UserService.Application.UseCases.Users.GetUsers;

public class GetUsersHandler(IRepositoryManager repManager, IMapper mapper, ICurrentUserService currentUserService, UserManager<User> userManager) 
	: IRequestHandler<GetUsersQuery, ApiBaseResponse>
{
	private readonly IRepositoryManager _repManager = repManager;
	private readonly IMapper _mapper = mapper;
	private readonly UserManager<User> _userManager = userManager;

	public async Task<ApiBaseResponse> Handle(GetUsersQuery request, CancellationToken cancellationToken)
	{
		var pagedUsers = await _repManager.Users.GetUsersAsync(request.Parameters, cancellationToken);

		var userDtos = new List<UserDetailsDto>();
		foreach (var user in pagedUsers)
		{
			var userDto = _mapper.Map<UserDetailsDto>(user);
			userDto.Roles = await _userManager.GetRolesAsync(user); 
			userDtos.Add(userDto);
		}

		var pagedResult = new PagedList<UserDetailsDto>(
			userDtos,
			pagedUsers.MetaData.TotalCount,
			pagedUsers.MetaData.CurrentPage,
			pagedUsers.MetaData.PageSize
		);

		return new ApiOkResponse<(List<UserDetailsDto> Items, MetaData MetaData)>((userDtos, pagedResult.MetaData));
	}
}