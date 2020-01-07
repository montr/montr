using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Metadata.Services
{
	public static class MetadataApplicationBuilderExtensions
	{
		public static IApplicationBuilder ConfigureMetadata(this IApplicationBuilder app, Action<MetadataOptions> action)
		{
			var serviceProvider = app.ApplicationServices;

			var options = new MetadataOptions
			{
				Registry = serviceProvider.GetRequiredService<IFieldProviderRegistry>()
			};

			action(options);

			return app;
		}
	}

	public class MetadataOptions
	{
		public IFieldProviderRegistry Registry { get; internal set; }
	}
}
