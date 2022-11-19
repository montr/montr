using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services.Implementations
{
	public class ApplicationEntityProvider : IEntityProvider
	{
		public async Task<object> GetEntity(string entityTypeCode, Guid entityUid, CancellationToken cancellationToken)
		{
			return await Task.FromResult(new Application());
		}
	}
}
