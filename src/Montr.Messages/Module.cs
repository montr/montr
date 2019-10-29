using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Messages.Services;

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
