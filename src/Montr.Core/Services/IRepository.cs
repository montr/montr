using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IRepository<TEntity>
	{
		Task<SearchResult<TEntity>> Search(SearchRequest searchRequest, CancellationToken cancellationToken);
	}
}
