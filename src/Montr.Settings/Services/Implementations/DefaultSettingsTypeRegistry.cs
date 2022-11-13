using System;
using System.Collections.Generic;

namespace Montr.Settings.Services.Implementations
{
	public class DefaultSettingsTypeRegistry : ISettingsTypeRegistry
	{
		private readonly List<Type> _types = new();
		
		public void Register(Type type)
		{
			_types.Add(type);
		}

		public IEnumerable<Type> GetRegisteredTypes()
		{
			return _types;
		}
	}
}