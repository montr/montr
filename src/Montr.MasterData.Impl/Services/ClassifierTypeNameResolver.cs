using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.Services
{
	public class ClassifierTypeNameResolver : IEntityNameResolver
	{
		private readonly IRepository<ClassifierType> _repository;

		public ClassifierTypeNameResolver(IRepository<ClassifierType> repository)
		{
			_repository = repository;
		}

		public async Task<string> Resolve(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new ClassifierTypeSearchRequest { Uid = entityUid }, cancellationToken);

			var entity = result?.Rows.SingleOrDefault();

			return entity?.Name;		}
	}
}
