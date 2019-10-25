using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Messages.Services;
using Montr.Modularity;

namespace Montr.Messages
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.Configure<EmailSenderOptions>(configuration.GetSection(EmailSenderOptions.SectionName));
		}
	}
}
