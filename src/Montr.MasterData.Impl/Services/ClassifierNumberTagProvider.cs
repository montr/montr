using System;
using System.Threading;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class ClassifierNumeratorTagProvider : INumeratorTagProvider
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
