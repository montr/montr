using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Montr.Core.Services
{
	public static class OptionsServiceCollectionExtensions
	{
		public static OptionsBuilder<TOptions> BindOptions<TOptions>(
			this IServiceCollection services, IConfiguration configuration) where TOptions : class
		{
			var sectionKey = OptionsUtils.GetOptionsSectionKey<TOptions>();

			return services
				.AddOptions<TOptions>()
				.Bind(configuration.GetSection(sectionKey))
				.ValidateDataAnnotations();
		}

		public static TOptions GetOptions<TOptions>(this IConfiguration configuration) where TOptions : class, new()
		{
			var sectionKey = OptionsUtils.GetOptionsSectionKey<TOptions>();

			return configuration.GetSection(sectionKey).Get<TOptions>() ?? new TOptions();
		}

		public static object GetOptions(this IConfiguration configuration, Type ofOptions)
		{
			var sectionKey = OptionsUtils.GetOptionsSectionKey(ofOptions);

			return configuration.GetSection(sectionKey).Get(ofOptions) ?? Activator.CreateInstance(ofOptions);
		}
	}
}
