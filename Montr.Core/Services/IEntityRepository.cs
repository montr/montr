using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IEntityRepository<TEntity>
	{
		Task<DataResult<TEntity>> Search(SearchRequest searchRequest, CancellationToken cancellationToken);
	}
}
