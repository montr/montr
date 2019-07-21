using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DefaultClassifierTreeService : IClassifierTreeService
	{
		private readonly IRepository<ClassifierTree> _repository;

		public DefaultClassifierTreeService(IRepository<ClassifierTree> repository)
		{
			_repository = repository;
		}

		public async Task<ClassifierTree> GetClassifierTree(Guid companyUid, string typeCode, string treeCode, CancellationToken cancellationToken)
		{
			var types = await _repository.Search(
				new ClassifierTreeSearchRequest
				{
					CompanyUid = companyUid,
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

		public async Task<ClassifierTree> GetClassifierTree(Guid companyUid, string typeCode, Guid treeUid, CancellationToken cancellationToken)
		{
			var types = await _repository.Search(
				new ClassifierTreeSearchRequest
				{
					CompanyUid = companyUid,
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