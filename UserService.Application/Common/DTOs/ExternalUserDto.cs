using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Common.DTOs;

public record ExternalUserDto(
	string Provider,
	string ProviderKey,
	string Email,
	string? FirstName,
	string? LastName);
