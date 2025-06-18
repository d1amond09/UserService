using UserService.Application.Common.RequestFeatures.ModelParameters;
using UserService.Application.Common.Responses;
using MediatR;
using UserService.Application.Common.RequestFeatures;
using UserService.Domain.Users;
using UserService.Application.Common.DTOs;

namespace UserService.Application.UseCases.Users.GetUsers;

public sealed record GetUsersQuery(UserParameters Parameters) : IRequest<PagedList<UserDetailsDto>>;
