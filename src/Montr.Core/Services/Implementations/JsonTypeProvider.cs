using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Montr.Core.Services.Implementations
{
	// ReSharper disable once UnusedTypeParameter
	// Generic parameter TBaseType is used to register different instances of JsonTypeProvider's
	public class JsonTypeProvider<TBaseType> : IJsonTypeProvider
	{
		private readonly List<Type> _types = new();

		public IDictionary<string, Type> Map { get; } =
			new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

		public void Register(string typeCode, Type type)
		{
			_types.Add(type);

			Map[typeCode] = type;
		}

		public bool IsRegistered(Type type)
		{
			return _types.Contains(type);
		}

		public bool TryGetType(string typeCode, out Type type)
		{
			return Map.TryGetValue(typeCode, out type);
		}
	}
}
