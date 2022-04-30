using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Montr.Core;
using Montr.Core.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl
{
	// ReSharper disable once UnusedType.Global
	public class Module : IModule, IAppConfigurator
	{
		public void Configure(IAppBuilder appBuilder)
		{
			appBuilder.Services.AddSingleton<IStartupTask, FieldMetadataJsonOptionsInitializer>();

			appBuilder.Services.AddSingleton<IFieldProviderRegistry, DefaultFieldProviderRegistry>();
			appBuilder.Services.AddSingleton<IRepository<FieldMetadata>, DbFieldMetadataRepository>();
			appBuilder.Services.AddSingleton<IFieldMetadataService, DbFieldMetadataService>();
			appBuilder.Services.AddSingleton<IFieldDataRepository, DbFieldDataRepository>();
			appBuilder.Services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
			appBuilder.Services.AddSingleton<IMetadataRegistrator, DefaultMetadataRegistrator>();

			appBuilder.Services.AddSingleton<JsonTypeProvider<FieldMetadata>>();
			appBuilder.Services.AddSingleton<IConfigureOptions<MvcNewtonsoftJsonOptions>, FieldMetadataJsonOptionsConfigurator>();
		}

		public void Configure(IApp app)
		{
			// todo: add phone, email, address, inn, bank info
			app.ConfigureMetadata(options =>
			{
				options.Registry.AddFieldType(typeof(SectionField));
				options.Registry.AddFieldType(typeof(TextField));
				options.Registry.AddFieldType(typeof(TextAreaField));
				options.Registry.AddFieldType(typeof(NumberField));
				options.Registry.AddFieldType(typeof(DecimalField));
				options.Registry.AddFieldType(typeof(DateField));
				// options.Registry.AddFieldType(typeof(TimeField)); // commented, problems with utc time without date
				options.Registry.AddFieldType(typeof(SelectField));
				options.Registry.AddFieldType(typeof(DesignSelectOptionsField));
				options.Registry.AddFieldType(typeof(FileField));
				options.Registry.AddFieldType(typeof(BooleanField));
				options.Registry.AddFieldType(typeof(PasswordField));
			});
		}
	}
}
