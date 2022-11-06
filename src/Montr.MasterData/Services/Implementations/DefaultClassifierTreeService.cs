using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.Implementations
{
	public class DefaultClassifierTreeService : IClassifierTreeService
	{
		private readonly IRepository<ClassifierTree> _repository;

		public DefaultClassifierTreeService(IRepository<ClassifierTree> repository)
		{
			_repository = repository;
		}

		public async Task<ClassifierTree> GetClassifierTree(string typeCode, string treeCode, CancellationToken cancellationToken)
		{
			var types = await _repository.Search(
				new ClassifierTreeSearchRequest
				{
					TypeCode = typeCode,
					Code = treeCode ?? throw new ArgumentNullException(nameof(treeCode)),
					PageNo = 0,
					PageSize = 2,
				}, cancellationToken);

			if (types.Rows.Count != 1)
			{
				throw new InvalidOperationException($"Classifier tree \"{treeCode}\" not found.");
			}

			return types.Rows.Single();
		}

		public async Task<ClassifierTree> GetClassifierTree(string typeCode, Guid treeUid, CancellationToken cancellationToken)
		{
			var types = await _repository.Search(
				new ClassifierTreeSearchRequest
				{
					TypeCode = typeCode,
					Uid = treeUid,
					PageNo = 0,
					PageSize = 2,
				}, cancellationToken);

			if (types.Rows.Count != 1)
			{
				throw new InvalidOperationException($"Classifier tree \"{treeUid}\" not found.");
			}

			return types.Rows.Single();
		}
	}
}
