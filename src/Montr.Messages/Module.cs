using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Messages.Services;
using Montr.Messages.Services.Implementations;

namespace Montr.Messages
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.BindOptions<SmtpOptions>(appBuilder.Configuration);

			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();
			appBuilder.Services.AddTransient<IContentProvider, ContentProvider>();

			appBuilder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();
			appBuilder.Services.AddSingleton<ITemplateRenderer, MustacheTemplateRenderer>();

			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbMessageTemplateRepository>(ClassifierTypeCode.MessageTemplate);
		}
	}
}
