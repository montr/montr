using System;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Montr.Web.Services
{
	public interface ICurrentUserProvider
	{
		T GetUserId<T>();

		Guid GetUserUid();
	}

	public class DefaultCurrentUserProvider : ICurrentUserProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DefaultCurrentUserProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public T GetUserId<T>()
		{
			var user = _httpContextAccessor.HttpContext.User;

			if (user.Identity.IsAuthenticated)
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
	}
}
