using AutoMapper;
using MediatR;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Exceptions;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Application.UseCases.Users.GetUserById;
using UserService.Domain.Common.Constants;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.IsUserBlocked;

public class IsUserBlockedHandler(IMapper mapper, IUserRepository userRep)
	: IRequestHandler<IsUserBlockedQuery, IsUserBlockedDto>
{
	public async Task<IsUserBlockedDto> Handle(IsUserBlockedQuery request, CancellationToken ct)
	{
		User user = await userRep.GetByIdAsync(request.UserId, ct)
			?? throw new NotFoundException("User not found.");

		var userDto = mapper.Map<IsUserBlockedDto>(user);

		return userDto;
	}
}

