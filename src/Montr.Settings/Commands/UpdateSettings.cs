using System;
using System.Security.Claims;
using MediatR;
using Montr.Core.Models;
using Montr.Settings.Services.Implementations;

namespace Montr.Settings.Commands
{
	public class UpdateSettings : IRequest<ApiResult>
	{
		public ClaimsPrincipal Principal { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string OptionsTypeCode { get; set; }

		[Newtonsoft.Json.JsonConverter(typeof(SettingsJsonConverterWrapper))]
		public object Values { get; set; }
	}
}
