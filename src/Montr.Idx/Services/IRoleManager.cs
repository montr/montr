using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Services
{
	public interface IRoleManager
	{
		Task<Role> Get(Guid roleUid, CancellationToken cancellationToken = default);

		Task<ApiResult> Create(Role role, CancellationToken cancellationToken = default);

		Task<ApiResult> Update(Role role, CancellationToken cancellationToken = default);

		Task<ApiResult> Delete(Role role, CancellationToken cancellationToken = default);
	}
}
