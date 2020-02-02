using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core.Services
{
	public static class OptionsServiceCollectionExtensions
	{
		public static IServiceCollection BindOptions<TOptions>(this IServiceCollection services, IConfiguration configuration) where TOptions : class
		{
			services
				.AddOptions<TOptions>()
				.Bind(configuration.GetSection(typeof(TOptions).FullName))
				.ValidateDataAnnotations();

			return services;
		}
	}
}
