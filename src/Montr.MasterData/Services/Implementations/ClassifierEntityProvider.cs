using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.Implementations
{
	public class ClassifierEntityProvider : IEntityProvider
	{
		private readonly INamedServiceFactory<IClassifierRepository> _classifierRepositoryFactory;

		public ClassifierEntityProvider(INamedServiceFactory<IClassifierRepository> classifierRepositoryFactory)
		{
			_classifierRepositoryFactory = classifierRepositoryFactory;
		}

		public async Task<object> GetEntity(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken)
		{
			// _classifierRepositoryFactory.GetRequiredService(typeCode)

			// todo: load from IClassifierRepository
			return await Task.FromResult(new Classifier());
		}
	}
}
