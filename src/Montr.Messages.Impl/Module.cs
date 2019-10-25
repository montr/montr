using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Messages.Impl.Services;
using Montr.Messages.Services;
using Montr.Modularity;

namespace Montr.Messages.Impl
{
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IEmailSender, MailKitEmailSender>();
			services.AddSingleton<ITemplateRenderer, MustacheTemplateRenderer>();
		}
	}
}
