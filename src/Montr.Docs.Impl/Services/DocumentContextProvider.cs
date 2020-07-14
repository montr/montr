using System;
using Montr.Core.Services;
using Montr.Docs.Models;

namespace Montr.Docs.Impl.Services
{
	public class DocumentContextProvider : IEntityContextProvider
	{
		public Type GetEntityType(string entityTypeCode, Guid entityTypeUid)
		{
			return typeof(Document);
		}
	}
}
