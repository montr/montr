using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
			services.AddTransient<IStartupTask, RegisterClassifierTypeStartupTask>();

			services.AddTransient<INumberGenerator, DbNumberGenerator>();

			services.AddTransient<IClassifierTypeProvider, ClassifierTypeProvider>();
			services.AddNamedTransient<IClassifierTypeProvider, NumeratorTypeProvider>(NumeratorTypeProvider.TypeCode);

			services.AddTransient<IRepository<ClassifierType>, DbClassifierTypeRepository>();
			services.AddTransient<IRepository<ClassifierTree>, DbClassifierTreeRepository>();
			services.AddTransient<IRepository<Classifier>, DbClassifierRepository>();

			services.AddTransient<IClassifierTypeRegistrator, DefaultClassifierTypeRegistrator>();
			services.AddTransient<IClassifierTypeService, DbClassifierTypeService>();
			services.AddTransient<IClassifierTypeMetadataService, ClassifierTypeMetadataService>();
			services.AddTransient<IClassifierTreeService, DefaultClassifierTreeService>();

			services.AddTransient<IRepository<Numerator>, DbNumeratorRepository>();
			services.AddTransient<INumberGenerator, DbNumberGenerator>();
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
