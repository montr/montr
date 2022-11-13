using System;
using System.Collections.Generic;

namespace Montr.Settings.Services
{
	public interface ISettingsTypeRegistry
	{
		void Register(Type type);

		IEnumerable<Type> GetRegisteredTypes();
	}
}
