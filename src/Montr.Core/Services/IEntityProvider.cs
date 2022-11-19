using System;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	// todo: convert to entity context provider (with status and other info) or create separate IEntityStatusProvider etc?
	public interface IEntityProvider
	{
		Task<object> GetEntity(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken);
	}
}
