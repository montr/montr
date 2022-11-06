using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Montr.Core.Services;
using Montr.Kompany.Entities;

namespace Montr.Kompany.Services.Implementations
{
	public class DefaultCurrentCompanyProvider : ICurrentCompanyProvider
	{
		private const string CookieName = "current_company_uid";

		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly ICurrentUserProvider _currentUserProvider;

		public DefaultCurrentCompanyProvider(IHttpContextAccessor httpContextAccessor, IDbContextFactory dbContextFactory,
			ICurrentUserProvider currentUserProvider)
		{
			_httpContextAccessor = httpContextAccessor;
			_dbContextFactory = dbContextFactory;
			_currentUserProvider = currentUserProvider;
		}

		public async Task<Guid> GetCompanyUid()
		{
			var userUid = _currentUserProvider.GetUserUid();

			var companyUid = GetCurrentCompanyUid();

			if (companyUid.HasValue)
			{
				using (var db = _dbContextFactory.Create())
				{
					if (await db.GetTable<DbCompanyUser>()
						.Where(x => x.CompanyUid == companyUid && x.UserUid == userUid)
						.Select(x => x.CompanyUid)
						.AnyAsync())
					{
						return companyUid.Value;
					}
				}
			}

			throw new InvalidOperationException("Company not found.");
		}

		private Guid? GetCurrentCompanyUid()
		{
			var cookieValue = _httpContextAccessor.HttpContext?.Request.Cookies[CookieName];

			if (cookieValue != null)
			{
				return Guid.Parse(cookieValue);
			}

			return null;
		}
	}
}
