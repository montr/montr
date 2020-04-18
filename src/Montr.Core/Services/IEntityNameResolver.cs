using System;

namespace Montr.Core.Services
{
	public interface IEntityNameResolver
	{
		string Resolve(string entityTypeCode, Guid entityUid);
	}
}
