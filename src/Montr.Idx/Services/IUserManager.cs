﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Services
{
	public interface IUserManager
	{
		Task<User> Get(Guid userUid, CancellationToken cancellationToken = default);

		Task<ApiResult> Create(User user, CancellationToken cancellationToken = default);

		Task<ApiResult> Create(User user, string password, CancellationToken cancellationToken = default);

		Task<ApiResult> Update(User user, CancellationToken cancellationToken = default);

		Task<ApiResult> Delete(User user, CancellationToken cancellationToken = default);

		Task<ApiResult> AddRoles(Guid userUid, IList<string> roles, CancellationToken cancellationToken = default);

		Task<ApiResult> RemoveRoles(Guid userUid, IList<string> roles, CancellationToken cancellationToken = default);
	}
}
