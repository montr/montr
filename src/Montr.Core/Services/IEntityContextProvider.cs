using System;

namespace Montr.Core.Services
{
	// todo: remove?
	public interface IEntityContextProvider
	{
		Type GetEntityType(string entityTypeCode, Guid entityTypeUid);
	}
}
