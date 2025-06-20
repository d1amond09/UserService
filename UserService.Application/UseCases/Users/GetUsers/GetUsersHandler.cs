using UserService.Application.Common.DTOs;
using UserService.Application.Common.RequestFeatures;
using MediatR;
using AutoMapper;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.GetUsers;

public class GetUsersHandler(IUserRepository userRep, IMapper mapper) 
	: IRequestHandler<GetUsersQuery, PagedList<UserDetailsDto>>
{
	private readonly IUserRepository _userRep = userRep;
	private readonly IMapper _mapper = mapper;

	public async Task<PagedList<UserDetailsDto>> Handle(GetUsersQuery request, CancellationToken ct)
	{
		var pagedUsers = await _userRep.GetAllAsync(request.Parameters, ct);

		var userDtos = _mapper.Map<List<UserDetailsDto>>(pagedUsers.Items);

		return new PagedList<UserDetailsDto>(userDtos, pagedUsers.MetaData);
	}
}