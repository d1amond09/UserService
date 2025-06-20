using UserService.Application.Common.RequestFeatures.ModelParameters;
using MediatR;
using UserService.Application.Common.RequestFeatures;
using UserService.Domain.Users;
using UserService.Application.Common.DTOs;
using UserService.Application.Common.Attributes;

namespace UserService.Application.UseCases.Users.GetUsers;

[Cacheable(1)]
public sealed record GetUsersQuery(UserParameters Parameters) : IRequest<PagedList<UserDetailsDto>>;
