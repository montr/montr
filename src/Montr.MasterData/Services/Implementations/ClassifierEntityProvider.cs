using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.MasterData.Services.Implementations
{
	public class ClassifierEntityProvider : IEntityProvider
	{
		public async Task<object> GetEntity(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken)
		{
			// todo: load from IClassifierRepository
			return await Task.FromResult(new Application());
		}
	}
}
