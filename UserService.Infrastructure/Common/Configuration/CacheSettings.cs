using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Common.Configuration;

public class CacheSettings
{
	public const string SectionName = "CacheSettings";
	public string ConnectionString { get; init; } = string.Empty;
	public string InstanceName { get; init; } = string.Empty;
}
