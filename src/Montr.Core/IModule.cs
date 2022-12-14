using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Montr.Core
{
	public interface IModule
	{
		void Configure(IAppBuilder appBuilder);
	}

	public interface IAppConfigurator
	{
		void Configure(IApp app);
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ModuleAttribute : Attribute
	{
		public Type[] DependsOn { get; set; }
	}

	public interface IAppBuilder
	{
		IWebHostEnvironment Environment { get; }

		ConfigurationManager Configuration { get; }

		IServiceCollection Services { get; }

		ICollection<IModule> Modules { get; }
	}

	// todo: move to impl?
	public class WebApplicationBuilderWrapper : IAppBuilder
	{
		private readonly WebApplicationBuilder _wrapped;

		public WebApplicationBuilderWrapper(WebApplicationBuilder wrapped, ICollection<IModule> modules)
		{
			_wrapped = wrapped;

			Modules = modules;
		}

		public IWebHostEnvironment Environment => _wrapped.Environment;

		public ConfigurationManager Configuration => _wrapped.Configuration;

		public IServiceCollection Services => _wrapped.Services;

		public ICollection<IModule> Modules { get; }
	}

	public interface IApp : IApplicationBuilder
	{
		ILogger Logger { get; }

		IWebHostEnvironment Environment { get; }

		IConfiguration Configuration { get; }

		IServiceProvider Services { get; }

		IHostApplicationLifetime Lifetime { get; }
	}

	// todo: move to impl?
	public class WebApplicationWrapper : IApp
	{
		private readonly WebApplication _wrapped;

		public WebApplicationWrapper(WebApplication wrapped)
		{
			_wrapped = wrapped;
		}

		public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
		{
			return _wrapped.Use(middleware);
		}

		public IApplicationBuilder New()
		{
			return ((IApplicationBuilder) _wrapped).New();
		}

		public RequestDelegate Build()
		{
			return ((IApplicationBuilder) _wrapped).Build();
		}

		public IServiceProvider ApplicationServices
		{
			get => ((IApplicationBuilder) _wrapped).ApplicationServices;
			set => ((IApplicationBuilder) _wrapped).ApplicationServices = value;
		}

		public IFeatureCollection ServerFeatures => ((IApplicationBuilder) _wrapped).ServerFeatures;

		public IDictionary<string, object> Properties => ((IApplicationBuilder) _wrapped).Properties;

		public ILogger Logger => _wrapped.Logger;

		public IWebHostEnvironment Environment => _wrapped.Environment;

		public IConfiguration Configuration => _wrapped.Configuration;

		public IServiceProvider Services => _wrapped.Services;

		public IHostApplicationLifetime Lifetime => _wrapped.Lifetime;
	}
}
