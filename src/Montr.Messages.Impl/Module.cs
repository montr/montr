using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Messages.Impl.Services;
using Montr.Messages.Services;

namespace Montr.Messages.Impl
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbMessageTemplateRepository>(ClassifierTypeCode.MessageTemplate);

			appBuilder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();
			appBuilder.Services.AddSingleton<ITemplateRenderer, MustacheTemplateRenderer>();
		}
	}
}
