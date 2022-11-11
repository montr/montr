using System;
using System.Security.Claims;
using MediatR;
using Montr.Core.Models;

namespace Montr.Settings.Commands
{
	public class UpdateSettings : IRequest<ApiResult>
	{
		public ClaimsPrincipal Principal { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string OptionsTypeCode { get; set; }

		public object Values { get; set; }
	}
}
