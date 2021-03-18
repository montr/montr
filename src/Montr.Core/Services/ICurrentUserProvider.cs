using System;
using System.Security.Claims;

namespace Montr.Core.Services
{
	public interface ICurrentUserProvider
	{
		T GetUserId<T>();

		Guid GetUserUid();

		ClaimsPrincipal GetUser(bool throwIfNotAuthenticated = true);
	}
}
