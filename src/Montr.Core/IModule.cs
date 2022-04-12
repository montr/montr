using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core
{
	public interface IModule
	{
	}

	public interface IWebApplicationBuilderConfigurator
	{
		void Configure(WebApplicationBuilder appBuilder);
	}

	public interface IWebApplicationConfigurator
	{
		void Configure(WebApplication app);
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ModuleAttribute : Attribute
	{
		public Type[] DependsOn { get; set; }
	}
}
