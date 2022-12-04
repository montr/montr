using System;
using System.Security.Claims;

namespace Montr.Core.Services
{
	public interface ICurrentUserProvider
	{
		Guid GetUserUid();

		Guid? GetUserUidIfAuthenticated();

		ClaimsPrincipal GetUser(bool throwIfNotAuthenticated = true);
	}
}
