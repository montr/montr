using System;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	// todo: remove - temp solution (?)
	public interface IMetadataRegistrator
	{
		void Register(string viewId, Func<string, DataView> getDataView);

		bool TryGet(string viewId, out DataView dataView);
	}
}
