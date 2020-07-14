using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IEntityStatusProvider
	{
		Task<ICollection<EntityStatus>> GetStatuses(EntityStatusSearchRequest request, CancellationToken cancellationToken);
	}
}
