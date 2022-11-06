using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services.Impl
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
