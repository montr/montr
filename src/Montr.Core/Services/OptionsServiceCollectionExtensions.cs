using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core.Services
{
	public static class OptionsServiceCollectionExtensions
	{
		public static IServiceCollection BindOptions<TOptions>(this IServiceCollection services, IConfiguration configuration) where TOptions : class
		{
			var sectionKey = typeof(TOptions).FullName;

			services
				.AddOptions<TOptions>()
				.Bind(configuration.GetSection(sectionKey))
				.ValidateDataAnnotations();

			return services;
		}

		public static TOptions GetOptions<TOptions>(this IConfiguration configuration) where TOptions : class
		{
			return configuration.GetSection(typeof(TOptions).FullName).Get<TOptions>();
		}
	}
}
