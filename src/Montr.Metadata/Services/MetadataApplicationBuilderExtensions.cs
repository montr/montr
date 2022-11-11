using System;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;

namespace Montr.Metadata.Services
{
	public static class MetadataApplicationBuilderExtensions
	{
		public static IApp ConfigureMetadata(this IApp app, Action<MetadataOptions> action)
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
		public IFieldProviderRegistry Registry { get; internal init; }
	}
}
