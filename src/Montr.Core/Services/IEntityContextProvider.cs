using System;

namespace Montr.Core.Services
{
	public interface IEntityContextProvider
	{
		Type GetEntityType(string entityTypeCode, Guid entityTypeUid);
	}
}
