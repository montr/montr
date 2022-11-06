using System;
using System.Collections.Concurrent;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services.Implementations
{
	public class DefaultMetadataRegistrator : IMetadataRegistrator
	{
		private readonly ConcurrentDictionary<string, Func<string, DataView>> _registry = new();

		public void Register(string viewId, Func<string, DataView> getDataView)
		{
			_registry[viewId] = getDataView;
		}

		public bool TryGet(string viewId, out DataView dataView)
		{
			if (viewId == null) throw new ArgumentNullException(nameof(viewId));

			if (_registry.TryGetValue(viewId, out var getDataView))
			{
				dataView = getDataView(viewId);
				return true;
			}

			dataView = null;
			return false;
		}
	}
}