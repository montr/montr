using System;
using System.Collections.Generic;

namespace Montr.Settings.Services
{
	public interface ISettingsTypeRegistry
	{
		void Register(Type type);

		bool TryGetType(string typeCode, out Type type);

		IEnumerable<(string, Type)> GetRegisteredTypes();
	}
}
