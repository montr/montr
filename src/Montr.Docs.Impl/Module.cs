using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;
using Montr.Docs.Impl.Services;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.MasterData.Services;

namespace Montr.Docs.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IModule, IWebApplicationBuilderConfigurator
	{
		public void Configure(WebApplicationBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<IContentProvider, ContentProvider>();

			appBuilder.Services.AddSingleton<IProcessService, DefaultProcessService>();

			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbDocumentTypeRepository>(ClassifierTypeCode.DocumentType);

			appBuilder.Services.AddSingleton<IRepository<Document>, DbDocumentRepository>();
			appBuilder.Services.AddSingleton<IDocumentService, DbDocumentService>();

			appBuilder.Services.AddTransient<INumberTagResolver, DocumentNumberTagResolver>();

			appBuilder.Services.AddNamedTransient<IRecipientResolver, DocumentRecipientResolver>(EntityTypeCode.Document);
			appBuilder.Services.AddNamedTransient<IEntityNameResolver, DocumentTypeNameResolver>(DocumentType.EntityTypeCode);
			appBuilder.Services.AddNamedTransient<IAutomationContextProvider, DocumentAutomationContextProvider>(MasterData.EntityTypeCode.Classifier);
		}
	}
}
