using System;
using System.Collections.Generic;
using System.Security.Claims;
using MediatR;
using Montr.Settings.Models;

namespace Montr.Settings.Queries
{
	public class GetSettingsMetadata : IRequest<ICollection<SettingsBlock>>
	{
		public ClaimsPrincipal Principal { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string SettingsTypeCode { get; set; }
	}
}
