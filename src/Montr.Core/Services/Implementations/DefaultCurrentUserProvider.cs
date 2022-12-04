using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Montr.Core.Services.Implementations
{
	public class DefaultCurrentUserProvider : ICurrentUserProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DefaultCurrentUserProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		private T GetUserId<T>(bool throwIfNotAuthenticated = true)
		{
			var user = _httpContextAccessor.HttpContext?.User;

			if (user?.Identity?.IsAuthenticated == true)
			{
				var sub = user.Claims.First(x => x.Type == "sub").Value;

				var typeConverter = TypeDescriptor.GetConverter(typeof(T));

				var result= (T)typeConverter.ConvertFromInvariantString(sub);

				return result;
			}

			if (throwIfNotAuthenticated)
			{
				throw new InvalidOperationException("User is not authenticated");
			}

			return default;
		}

		public Guid GetUserUid()
		{
			return GetUserId<Guid>();
		}

		public Guid? GetUserUidIfAuthenticated()
		{
			return GetUserId<Guid?>(throwIfNotAuthenticated: false);
		}

		public ClaimsPrincipal GetUser(bool throwIfNotAuthenticated = true)
		{
			var user = _httpContextAccessor.HttpContext?.User;

			if (user?.Identity?.IsAuthenticated == true)
			{
				return user;
			}

			if (throwIfNotAuthenticated)
			{
				throw new InvalidOperationException("User is not authenticated");
			}

			return null;
		}
	}
}
