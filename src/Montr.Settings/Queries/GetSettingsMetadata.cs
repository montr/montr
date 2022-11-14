using System;
using System.Security.Claims;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Settings.Queries
{
	public class GetSettingsMetadata : IRequest<DataView>
	{
		public ClaimsPrincipal Principal { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string SettingsTypeCode { get; set; }
	}
}
