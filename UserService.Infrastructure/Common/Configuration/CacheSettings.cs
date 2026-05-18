using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Infrastructure.Common.Configuration;

public class CacheSettings
{
	public const string SectionName = "CacheSettings";
	public string InstanceName { get; set; } = "MyApplication:";
	public string Host { get; set; } = "localhost";
	public int Port { get; set; } = 6379;
	public string User { get; set; } = "default"; 
	public string Password { get; set; } = "";
}
