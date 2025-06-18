using UserService.Application.Common.DTOs;
using UserService.Domain.Users;
using AutoMapper;

namespace UserService.Application.Common;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<User, UserDtos>();
		CreateMap<UserForRegistrationDto, User>();

		CreateMap<User, UserSummaryDto>();
		CreateMap<User, UserDetailsDto>()
			.ForMember(dest => dest.Roles, opt => opt.Ignore())
			.ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));
	}
}

