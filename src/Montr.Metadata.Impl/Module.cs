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
				options.Registry.AddFieldType(BooleanField.TypeCode, new DefaultFieldProvider<BooleanField, bool>());
				options.Registry.AddFieldType(TextField.TypeCode, new DefaultFieldProvider<TextField, string>());
				options.Registry.AddFieldType(TextAreaField.TypeCode, new DefaultFieldProvider<TextAreaField, string>());
				options.Registry.AddFieldType(PasswordField.TypeCode, new DefaultFieldProvider<PasswordField, string>());
				options.Registry.AddFieldType(NumberField.TypeCode, new DefaultFieldProvider<NumberField, long>());
				options.Registry.AddFieldType(DecimalField.TypeCode, new DefaultFieldProvider<DecimalField, decimal>());
				options.Registry.AddFieldType(DateField.TypeCode, new DateFieldProvider());
				options.Registry.AddFieldType(TimeField.TypeCode, new TimeFieldProvider());
				options.Registry.AddFieldType(SelectField.TypeCode, new DefaultFieldProvider<SelectField, string>());
				options.Registry.AddFieldType(DesignSelectOptionsField.TypeCode, new DefaultFieldProvider<DesignSelectOptionsField, SelectFieldOption>());
				// todo: move classifier fields to MasterData
				options.Registry.AddFieldType(ClassifierField.Code, new DefaultFieldProvider<ClassifierField, string>());
				options.Registry.AddFieldType(ClassifierGroupField.Code, new DefaultFieldProvider<ClassifierGroupField, string>());
				options.Registry.AddFieldType(FileField.TypeCode, new DefaultFieldProvider<FileField, string>());
			});
		}
	}
}
