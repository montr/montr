using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Montr.Core.Services
{
	public interface INamedServiceFactory<out TService>
	{
		TService GetService(string name);

		TService GetRequiredService(string name);

		IEnumerable<string> GetNames();
	}

	public class DefaultNamedServiceFactory<TService> : INamedServiceFactory<TService>
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly IDictionary<string, Type> _registrations;

		public DefaultNamedServiceFactory(IServiceProvider serviceProvider, IDictionary<string, Type> registrations)
		{
			_serviceProvider = serviceProvider;
			_registrations = registrations;
		}

		public TService GetService(string name)
		{
			if (_registrations.TryGetValue(name, out var serviceType))
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
				throw new ArgumentException($"Service with name {name} is not registered.");
			}

			return service;
		}

		public IEnumerable<string> GetNames()
		{
			return _registrations.Keys;
		}
	}

	/// <summary>
	/// Registration of named services, for example:
	/// services.AddNamedTransient<INumeratorTagProvider, ClassifierNumeratorTagProvider>(ClassifierType.EntityTypeCode);
	/// </summary>
	public static class NamedServiceCollectionExtensions
	{
		private static readonly ConcurrentDictionary<Type, IDictionary<string, Type>>
			FactoryRegistrations = new ConcurrentDictionary<Type, IDictionary<string, Type>>();

		public static IServiceCollection AddNamedTransient<TService, TImplementation>(this IServiceCollection services, string name)
			where TService : class
			where TImplementation : class, TService
		{
			var factoryType = typeof(INamedServiceFactory<>).MakeGenericType(typeof(TService));

			var factoryRegistrations = FactoryRegistrations.GetOrAdd(factoryType, _ =>
			{
				var registrations = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

				services.AddTransient<INamedServiceFactory<TService>>(
					serviceProvider => new DefaultNamedServiceFactory<TService>(serviceProvider, registrations));

				return registrations;
			});

			factoryRegistrations.Add(name, typeof(TImplementation));

			services.AddTransient<TImplementation>();

			return services;
		}
	}
}
