using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultCurrentUserProvider : ICurrentUserProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DefaultCurrentUserProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public T GetUserId<T>()
		{
			var user = _httpContextAccessor.HttpContext?.User;

			if (user?.Identity?.IsAuthenticated == true)
			{
				var sub = user.Claims.First(x => x.Type == "sub").Value;

				var typeConverter = TypeDescriptor.GetConverter(typeof(T));

				var result= (T)typeConverter.ConvertFromInvariantString(sub);

				return result;
			}

			throw new InvalidOperationException("User is not authenticated");
		}

		public Guid GetUserUid()
		{
			return GetUserId<Guid>();
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
