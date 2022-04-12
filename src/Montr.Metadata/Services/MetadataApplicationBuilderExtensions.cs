using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Metadata.Services
{
	public static class MetadataApplicationBuilderExtensions
	{
		public static IApplicationBuilder ConfigureMetadata(this WebApplication app, Action<MetadataOptions> action)
		{
			var options = new MetadataOptions
			{
				Registry = app.Services.GetRequiredService<IFieldProviderRegistry>()
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
