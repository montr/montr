using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Montr.Core.Services
{
	public class JsonTypeProvider<TBaseType>
	{
		public IDictionary<string, Type> Map { get; } = new ConcurrentDictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
	}
}
