using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Services;
using Montr.Core.Services.Implementations;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.MasterData.Services.Implementations;
using Montr.Metadata.Services;
using PermissionProvider = Montr.MasterData.Services.Implementations.PermissionProvider;

namespace Montr.MasterData
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule, IAppConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierMetadataStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();
			appBuilder.Services.AddTransient<IStartupTask, ConfigurationStartupTask>();

			appBuilder.Services.AddSingleton<IStartupTask, ClassifierJsonOptionsInitializer>();

			appBuilder.Services.AddSingleton<IContentProvider, ContentProvider>();
			appBuilder.Services.AddTransient<IPermissionProvider, PermissionProvider>();

			appBuilder.Services.AddTransient<INumberGenerator, DbNumberGenerator>();

			appBuilder.Services.AddTransient<IClassifierRepository, DbClassifierRepository<Classifier>>();
			appBuilder.Services.AddNamedTransient<IClassifierRepository, DbNumeratorRepository>(ClassifierTypeCode.Numerator);

			appBuilder.Services.AddNamedTransient<IEntityNameResolver, ClassifierTypeNameResolver>(ClassifierType.TypeCode);
			appBuilder.Services.AddNamedTransient<IEntityProvider, ClassifierEntityProvider>(EntityTypeCode.Classifier);

			appBuilder.Services.AddNamedTransient<IRecipeStepHandler, RegisterClassifierTypeStep>(RegisterClassifierTypeStep.Name);

			appBuilder.Services.AddTransient<IRepository<ClassifierType>, DbClassifierTypeRepository>();
			appBuilder.Services.AddTransient<IRepository<ClassifierTree>, DbClassifierTreeRepository>();
			appBuilder.Services.AddTransient<IRepository<NumeratorEntity>, DbNumeratorEntityRepository>();

			appBuilder.Services.AddTransient<IClassifierTypeRegistrator, DefaultClassifierTypeRegistrator>();
			appBuilder.Services.AddTransient<IClassifierRegistrator, DefaultClassifierRegistrator>();

			appBuilder.Services.AddTransient<IClassifierTypeService, DbClassifierTypeService>();
			appBuilder.Services.AddTransient<IClassifierTypeMetadataService, ClassifierTypeMetadataService>();
			appBuilder.Services.AddTransient<IClassifierTreeService, DefaultClassifierTreeService>();

			appBuilder.Services.AddTransient<INumberGenerator, DbNumberGenerator>();

			appBuilder.Services.AddSingleton<JsonTypeProvider<Classifier>>();
			appBuilder.Services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, ClassifierJsonOptionsConfigurator>();
		}

		public void Configure(IApp app)
		{
			app.ConfigureMetadata(options =>
			{
				options.Registry.AddFieldType(typeof(ClassifierField));
				options.Registry.AddFieldType(typeof(ClassifierGroupField));
				options.Registry.AddFieldType(typeof(ClassifierTypeField));
			});
		}
	}
}
