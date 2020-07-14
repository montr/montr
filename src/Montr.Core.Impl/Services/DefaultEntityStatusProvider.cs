using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultEntityStatusProvider : IEntityStatusProvider
	{
		private readonly IRepository<EntityStatus> _repository;

		public DefaultEntityStatusProvider(IRepository<EntityStatus> repository)
		{
			_repository = repository;
		}

		public async Task<ICollection<EntityStatus>> GetStatuses(EntityStatusSearchRequest request, CancellationToken cancellationToken)
		{
			// todo: cache

			var result = await _repository.Search(request, cancellationToken);

			return result.Rows;
		}
	}
}
