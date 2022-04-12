using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Messages.Impl.Services;
using Montr.Messages.Services;

namespace Montr.Messages.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IWebApplicationBuilderConfigurator
	{
		public void Configure(WebApplicationBuilder appBuilder)
		{
			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbMessageTemplateRepository>(ClassifierTypeCode.MessageTemplate);

			appBuilder.Services.AddSingleton<IEmailSender, MailKitEmailSender>();
			appBuilder.Services.AddSingleton<ITemplateRenderer, MustacheTemplateRenderer>();
		}
	}
}
