using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Montr.Core.Services;

namespace Montr.Settings.Services.Implementations
{
	public class DefaultSettingsTypeRegistry : ISettingsTypeRegistry
	{
		private readonly ConcurrentDictionary<string, Type> _types = new();

		public void Register(Type type)
		{
			var typeCode = OptionsUtils.GetOptionsSectionKey(type);

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
