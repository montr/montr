using Microsoft.Extensions.DependencyInjection;
using Montr.Automate.Services;
using Montr.Core;
using Montr.Core.Services;
using Montr.Docs.Models;
using Montr.Docs.Services;
using Montr.Docs.Services.Implementations;
using Montr.MasterData.Services;
using ConfigurationStartupTask = Montr.Docs.Services.Implementations.ConfigurationStartupTask;
using RegisterClassifierTypeStartupTask = Montr.Docs.Services.Implementations.RegisterClassifierTypeStartupTask;

namespace Montr.Docs;

// ReSharper disable once UnusedType.Global
public class Module : IModule
{
	public void Configure(IAppBuilder appBuilder)
	{
		appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
		appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();

		appBuilder.Services.AddSingleton<IContentProvider, ContentProvider>();

		appBuilder.Services.AddSingleton<IProcessService, DefaultProcessService>();

		appBuilder.Services
			.AddNamedTransient<IClassifierRepository, DbDocumentTypeRepository>(ClassifierTypeCode.DocumentType);

		appBuilder.Services.AddSingleton<IRepository<Document>, DbDocumentRepository>();
		appBuilder.Services.AddTransient<IDocumentService, DbDocumentService>();

		appBuilder.Services.AddTransient<INumberTagResolver, DocumentNumberTagResolver>();

		appBuilder.Services.AddNamedTransient<IEntityNameResolver, DocumentTypeNameResolver>(DocumentType.EntityTypeCode);

		appBuilder.Services.AddNamedTransient<IRecipientResolver, DocumentRecipientResolver>(EntityTypeCode.Document);
		appBuilder.Services.AddNamedTransient<IAutomationContextProvider, DocumentAutomationContextProvider>(EntityTypeCode.Document);
	}
}
