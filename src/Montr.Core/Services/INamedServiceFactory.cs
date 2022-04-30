using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Montr.Core.Services
{
	public interface INamedServiceFactory<out TService>
	{
		IEnumerable<string> GetNames();

		TService GetService(string name);

		TService GetRequiredService(string name);

		TService GetNamedOrDefaultService(string name);
	}

	// ReSharper disable once UnusedTypeParameter - type parameter used to register and resolve service
	public class NamedServiceTypeMapper<TService>
	{
		private readonly IDictionary<string, Type> _registrations =
			new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

		public void MapTypeToName(string name, Type type)
		{
			_registrations[name] = type;
		}

		public bool TryGetTypeByName(string name, out Type type)
		{
			return _registrations.TryGetValue(name, out type);
		}

		public IEnumerable<string> GetNames()
		{
			return _registrations.Keys;
		}
	}

	public interface IConfigureNamedServiceTypeMapper
	{
		public void Configure();
	}

	public class ConfigureNamedServiceTypeMapper<TService> : IConfigureNamedServiceTypeMapper
	{
		private readonly NamedServiceTypeMapper<TService> _mapper;
		private readonly string _name;
		private readonly Type _implementation;

		public ConfigureNamedServiceTypeMapper(NamedServiceTypeMapper<TService> mapper, string name, Type implementation)
		{
			_mapper = mapper;
			_name = name;
			_implementation = implementation;
		}

		public void Configure()
		{
			_mapper.MapTypeToName(_name, _implementation);
		}
	}

	public class DefaultNamedServiceFactory<TService> : INamedServiceFactory<TService>
	{
		private readonly NamedServiceTypeMapper<TService> _serviceTypeMapper;
		private readonly IServiceProvider _serviceProvider;

		public DefaultNamedServiceFactory(NamedServiceTypeMapper<TService> serviceTypeMapper, IServiceProvider serviceProvider)
		{
			_serviceTypeMapper = serviceTypeMapper;
			_serviceProvider = serviceProvider;
		}

		public IEnumerable<string> GetNames()
		{
			return _serviceTypeMapper.GetNames();
		}

		public TService GetService(string name)
		{
			if (_serviceTypeMapper.TryGetTypeByName(name, out var serviceType))
			{
				return (TService)_serviceProvider.GetRequiredService(serviceType);
			}

			return default;
		}

		public TService GetRequiredService(string name)
		{
			var service = GetService(name);

			if (service == null)
			{
				throw new ArgumentException($"Service with name \"{name}\" is not registered.");
			}

			return service;
		}

		public TService GetNamedOrDefaultService(string name)
		{
			var service = GetService(name);

			if (service == null)
			{
				return _serviceProvider.GetRequiredService<TService>();
			}

			return service;
		}
	}

	/// <summary>
	/// Registration of named services, for example:
	/// services.AddNamedTransient&lt;INumeratorTagProvider, ClassifierNumeratorTagProvider>(ClassifierType.EntityTypeCode);
	/// </summary>
	public static class NamedServiceCollectionExtensions
	{
		public static IServiceCollection AddNamedTransient<TService, TImplementation>(this IServiceCollection services, string name)
			where TService : class
			where TImplementation : class, TService
		{
			// try add (if not already) single instance that holds name to implementation type mapping
			services.TryAddSingleton<NamedServiceTypeMapper<TService>>();

			// try add (if not already) named factory for given service
			services.TryAddTransient<INamedServiceFactory<TService>, DefaultNamedServiceFactory<TService>>();

			// for each registered implementation register its mapping
			services.AddTransient<IConfigureNamedServiceTypeMapper>(serviceProvider => ActivatorUtilities
				.CreateInstance<ConfigureNamedServiceTypeMapper<TService>>(serviceProvider, name, typeof(TImplementation)));

			// add implementation
			services.AddTransient<TImplementation>();

			return services;
		}

		public static void UseNamedServices(this IServiceProvider serviceProvider)
		{
			var configurators = serviceProvider.GetServices<IConfigureNamedServiceTypeMapper>();

			foreach (var configurator in configurators)
			{
				configurator.Configure();
			}
		}
	}
}
