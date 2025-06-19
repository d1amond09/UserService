using UserService.Application.Common.DTOs;
using UserService.Application.Common.RequestFeatures;
using MediatR;
using AutoMapper;
using UserService.Application.Common.Interfaces;
using UserService.Domain.Users;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Interfaces.Persistence;

namespace UserService.Application.UseCases.Users.GetUserMe;

public class GetUserMeHandler(IMapper mapper, ICurrentUserService currentUserService, IUserRepository userRep) 
	: IRequestHandler<GetUserMeQuery, UserDetailsDto>
{
	private readonly IMapper _mapper = mapper;
	private readonly ICurrentUserService _currentUserService = currentUserService;
	private readonly IUserRepository _userRep = userRep; 

	public async Task<UserDetailsDto> Handle(GetUserMeQuery request, CancellationToken ct)
	{
		Guid userId = _currentUserService.UserId 
			?? throw new UnauthorizedAccessException();

		User user = await _userRep.GetByIdAsync(userId, ct)
			?? throw new NotFoundException("User not found.");

		return _mapper.Map<UserDetailsDto>(user);
	}
}