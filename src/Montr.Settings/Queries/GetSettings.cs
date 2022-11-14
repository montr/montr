using System;
using System.Security.Claims;
using MediatR;
using Montr.Core.Models;

namespace Montr.Settings.Queries
{
	public class GetSettings : IRequest<ApiResult<object>>
	{
		public ClaimsPrincipal Principal { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string SettingsTypeCode { get; set; }
	}
}
