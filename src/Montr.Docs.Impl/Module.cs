﻿using Microsoft.Extensions.Configuration;
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
	public class Module : IModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IContentProvider, ContentProvider>();

			services.AddSingleton<IProcessService, DefaultProcessService>();

			services.AddNamedTransient<IClassifierRepository, DbDocumentTypeRepository>(ClassifierTypeCode.DocumentType);

			services.AddSingleton<IRepository<Document>, DbDocumentRepository>();
			services.AddSingleton<IDocumentService, DbDocumentService>();

			services.AddTransient<INumberTagResolver, DocumentNumberTagResolver>();

			services.AddNamedTransient<IRecipientResolver, DocumentRecipientResolver>(DocumentType.EntityTypeCode);
			services.AddNamedTransient<IEntityNameResolver, DocumentTypeNameResolver>(DocumentType.EntityTypeCode);
			services.AddNamedTransient<IAutomationContextProvider, DocumentAutomationContextProvider>(DocumentType.EntityTypeCode);
		}
	}
}
