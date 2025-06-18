using AutoMapper;
using MediatR;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Application.Common.Interfaces;
using UserService.Application.UseCases.Users.GetUserMe;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Users;

namespace UserService.Application.UseCases.Users.GetUserById;

public class GetUserByIdHandler(IMapper mapper, IUserRepository userRep)
	: IRequestHandler<GetUserByIdQuery, UserDetailsDto>
{
	private readonly IMapper _mapper = mapper;
	private readonly IUserRepository _userRep = userRep;

	public async Task<UserDetailsDto> Handle(GetUserByIdQuery request, CancellationToken ct)
	{
		User user = await _userRep.GetByIdAsync(request.UserId, ct)
			?? throw new NotFoundException("User not found.");

		return _mapper.Map<UserDetailsDto>(user);
	}
}
