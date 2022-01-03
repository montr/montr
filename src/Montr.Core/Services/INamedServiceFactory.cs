using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core.Services
{
	public interface INamedServiceFactory<out TService>
	{
		IEnumerable<string> GetNames();

		TService GetService(string name);

		TService GetRequiredService(string name);

		TService GetNamedOrDefaultService(string name);
	}

	public class DefaultNamedServiceFactory<TService> : INamedServiceFactory<TService>
	{
		private readonly IServiceProvider _serviceProvider;

		public IDictionary<string, Type> Registrations { get; set; }

		public DefaultNamedServiceFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public IEnumerable<string> GetNames()
		{
			return Registrations.Keys;
		}

		public TService GetService(string name)
		{
			if (Registrations.TryGetValue(name, out var serviceType))
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
		private static readonly ConcurrentDictionary<Type, IDictionary<string, Type>> FactoryRegistrations = new();

		public static IServiceCollection AddNamedTransient<TService, TImplementation>(this IServiceCollection services, string name)
			where TService : class
			where TImplementation : class, TService
		{
			var factoryType = typeof(INamedServiceFactory<>).MakeGenericType(typeof(TService));

			var factoryRegistrations = FactoryRegistrations.GetOrAdd(factoryType, _ =>
			{
				var registrations = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

				services.AddTransient<INamedServiceFactory<TService>>(serviceProvider =>
				{
					var factory = ActivatorUtilities.GetServiceOrCreateInstance<DefaultNamedServiceFactory<TService>>(serviceProvider);

					factory.Registrations = registrations;

					return factory;
				});

				return registrations;
			});

			factoryRegistrations.Add(name, typeof(TImplementation));

			services.AddTransient<TImplementation>();

			return services;
		}

		public static void ClearRegistrations()
		{
			FactoryRegistrations.Clear();
		}
	}
}
