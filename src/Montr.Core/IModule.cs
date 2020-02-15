using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core
{
	public interface IModule
	{
		void ConfigureServices(IConfiguration configuration, IServiceCollection services);
	}

	public interface IWebModule : IModule
	{
		void Configure(IApplicationBuilder app);
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ModuleAttribute : Attribute
	{
		public Type ImplementationOf { get; set; }

		public Type[] DependsOn { get; set; }
	}

	public enum ModuleType : byte
	{
		Interface = 0,
		Implementation = 1,
		Plugin = 2
	}
}
