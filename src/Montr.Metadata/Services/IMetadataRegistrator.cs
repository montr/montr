using System;
using System.Collections.Concurrent;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	// todo: remove - temp solution
	public interface IMetadataRegistrator
	{
		void Register(string viewId, Func<string, DataView> getDataView);

		bool TryGet(string viewId, out DataView dataView);
	}

	public class DefaultMetadataRegistrator : IMetadataRegistrator
	{
		private readonly ConcurrentDictionary<string, Func<string, DataView>> _registry = new ConcurrentDictionary<string, Func<string, DataView>>();

		public void Register(string viewId, Func<string, DataView> getDataView)
		{
			_registry[viewId] = getDataView;
		}

		public bool TryGet(string viewId, out DataView dataView)
		{
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
