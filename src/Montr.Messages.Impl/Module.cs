using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Messages.Impl.Services;
using Montr.Messages.Models;
using Montr.Messages.Services;

namespace Montr.Messages.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddNamedTransient<IClassifierRepository, DbMessageTemplateRepository>(MessageTemplate.TypeCode);

			services.AddSingleton<IEmailSender, MailKitEmailSender>();
			services.AddSingleton<ITemplateRenderer, MustacheTemplateRenderer>();

			services.AddTransient<IMessageTemplateRegistrator, DefaultMessageTemplateRegistrator>();
		}
	}
}
