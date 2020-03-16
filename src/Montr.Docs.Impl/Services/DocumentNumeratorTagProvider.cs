using System;
using System.Threading;
using Montr.MasterData.Services;

namespace Montr.Docs.Impl.Services
{
	public class DocumentNumeratorTagProvider : INumeratorTagProvider
	{
		public string[] GetAvailableTags()
		{
			throw new NotImplementedException();
		}

		public void ResolveValues(string entityTypeCode, Guid enityUid, string[] tags, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
