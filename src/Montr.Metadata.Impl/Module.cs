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
	public class Module : IWebModule
	{
		public void ConfigureServices(IConfiguration configuration, IServiceCollection services)
		{
			services.AddSingleton<IFieldProviderRegistry, DefaultFieldProviderRegistry>();
			services.AddSingleton<IRepository<FieldMetadata>, DbFieldMetadataRepository>();
			services.AddSingleton<IFieldDataRepository, DbFieldDataRepository>();
			services.AddSingleton<IMetadataProvider, DefaultMetadataProvider>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.ConfigureMetadata(options =>
			{
				options.Registry.AddFieldType(BooleanField.TypeCode, new DefaultFieldProvider<BooleanField>());
				options.Registry.AddFieldType(TextField.TypeCode, new DefaultFieldProvider<TextField>());
				options.Registry.AddFieldType(TextAreaField.TypeCode, new DefaultFieldProvider<TextAreaField>());
				options.Registry.AddFieldType(PasswordField.TypeCode, new DefaultFieldProvider<PasswordField>());
				options.Registry.AddFieldType(NumberField.TypeCode, new DefaultFieldProvider<NumberField>());
				options.Registry.AddFieldType(DecimalField.TypeCode, new DefaultFieldProvider<DecimalField>());
				options.Registry.AddFieldType(DateField.TypeCode, new DefaultFieldProvider<DateField>());
				options.Registry.AddFieldType(TimeField.TypeCode, new DefaultFieldProvider<TimeField>());
				options.Registry.AddFieldType(SelectField.TypeCode, new DefaultFieldProvider<SelectField>());
				// todo: move classifier fields to MasterData
				options.Registry.AddFieldType(ClassifierField.Code, new DefaultFieldProvider<ClassifierField>());
				options.Registry.AddFieldType(ClassifierGroupField.Code, new DefaultFieldProvider<ClassifierGroupField>());
				options.Registry.AddFieldType(FileField.TypeCode, new DefaultFieldProvider<FileField>());
			});
		}
	}
}
