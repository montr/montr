using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Services;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IStartupTask, ClassifierJsonOptionsInitializer>();

			services.AddSingleton<IContentProvider, ContentProvider>();
			services.AddTransient<IPermissionProvider, PermissionProvider>();

			services.AddTransient<INumberGenerator, DbNumberGenerator>();

			services.AddTransient<IClassifierRepository, DbClassifierRepository<Classifier>>();
			services.AddNamedTransient<IClassifierRepository, DbNumeratorRepository>(ClassifierTypeCode.Numerator);

			services.AddNamedTransient<IEntityNameResolver, ClassifierTypeNameResolver>(ClassifierType.TypeCode);

			services.AddNamedTransient<IRecipeStepHandler, RegisterClassifierTypeStep>(RegisterClassifierTypeStep.Name);

			services.AddTransient<IRepository<ClassifierType>, DbClassifierTypeRepository>();
			services.AddTransient<IRepository<ClassifierTree>, DbClassifierTreeRepository>();
			services.AddTransient<IRepository<NumeratorEntity>, DbNumeratorEntityRepository>();

			services.AddTransient<IClassifierTypeRegistrator, DefaultClassifierTypeRegistrator>();
			services.AddTransient<IClassifierRegistrator, DefaultClassifierRegistrator>();

			services.AddTransient<IClassifierTypeService, DbClassifierTypeService>();
			services.AddTransient<IClassifierTypeMetadataService, ClassifierTypeMetadataService>();
			services.AddTransient<IClassifierTreeService, DefaultClassifierTreeService>();

			services.AddTransient<INumberGenerator, DbNumberGenerator>();

			services.AddSingleton<JsonTypeProvider<Classifier>>();
			services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, ClassifierJsonOptionsConfigurator>();
		}

		public void Configure(IApplicationBuilder app)
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
