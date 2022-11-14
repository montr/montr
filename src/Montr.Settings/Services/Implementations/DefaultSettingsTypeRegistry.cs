using System;
using System.Collections.Generic;
using System.Linq;

namespace Montr.Settings.Services.Implementations
{
	public class DefaultSettingsTypeRegistry : ISettingsTypeRegistry
	{
		private readonly Dictionary<string, Type> _types = new();

		public string GetTypeCode(Type type)
		{
			return type.FullName;
		}

		public void Register(Type type)
		{
			var typeCode = GetTypeCode(type);

			_types[typeCode] = type;
		}

		public bool TryGetType(string typeCode, out Type type)
		{
			return _types.TryGetValue(typeCode, out type);
		}

		public IEnumerable<(string, Type)> GetRegisteredTypes()
		{
			return _types.Select(x => (x.Key, x.Value));
		}
	}
}
