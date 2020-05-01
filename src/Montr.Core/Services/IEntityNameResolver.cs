using System;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	public interface IEntityNameResolver
	{
		Task<string> Resolve(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken);
	}
}
