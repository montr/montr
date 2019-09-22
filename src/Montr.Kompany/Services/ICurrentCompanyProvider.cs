using System;
using Microsoft.AspNetCore.Http;

namespace Montr.Kompany.Services
{
	public interface ICurrentCompanyProvider
	{
		Guid GetCompanyUid();
	}

	public class DefaultCurrentCompanyProvider : ICurrentCompanyProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public DefaultCurrentCompanyProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public Guid GetCompanyUid()
		{
			var cookieValue = _httpContextAccessor.HttpContext.Request.Cookies["current_company_uid"];

			if (cookieValue != null)
			{
				return Guid.Parse(cookieValue);
			}

			throw new InvalidOperationException("Company is not exists");
		}
	}
}
