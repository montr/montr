using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Montr.Core;
using Montr.Core.Services;
using Montr.Metadata.Impl.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.Metadata.Impl
{
	// ReSharper disable once UnusedMember.Global
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IFieldProviderRegistry, DefaultFieldProviderRegistry>();
			services.AddSingleton<IRepository<FieldMetadata>, DbFieldMetadataRepository>();
			services.AddSingleton<IFieldMetadataService, DbFieldMetadataService>();
			services.AddSingleton<IFieldDataRepository, DbFieldDataRepository>();
			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
			services.AddSingleton<IMetadataRegistrator, DefaultMetadataRegistrator>();
		}

		public void Configure(IApplicationBuilder app)
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
				// options.Registry.AddFieldType(typeof(TimeField)); commented, problems with utc time without date
				options.Registry.AddFieldType(typeof(SelectField));
				options.Registry.AddFieldType(typeof(DesignSelectOptionsField));
				options.Registry.AddFieldType(typeof(FileField));
				options.Registry.AddFieldType(typeof(BooleanField));
				options.Registry.AddFieldType(typeof(PasswordField));
			});
		}
	}
}
