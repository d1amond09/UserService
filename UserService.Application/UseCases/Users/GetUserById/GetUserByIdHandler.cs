using AutoMapper;
using MediatR;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Interfaces.Persistence;
using UserService.Application.Common.Interfaces;
using UserService.Application.UseCases.Users.GetUserMe;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Users;
using UserService.Domain.Common.Constants;

namespace UserService.Application.UseCases.Users.GetUserById;

public class GetUserByIdHandler(IMapper mapper, IUserRepository userRep, IPictureRepository picRep)
	: IRequestHandler<GetUserByIdQuery, UserDetailsDto>
{
	public async Task<UserDetailsDto> Handle(GetUserByIdQuery request, CancellationToken ct)
	{
		User user = await userRep.GetByIdAsync(request.UserId, ct)
			?? throw new NotFoundException("User not found.");

		var userDto = mapper.Map<UserDetailsDto>(user);

		Picture picture = await picRep.GetByIdAsync(user.PictureId ?? Guid.Parse(Pictures.DefaultId))
			?? throw new NotFoundException("Picture was not found.");

		userDto.PictureUrl = picture.Url;
		return userDto;
	}
}
