using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MediatR;
using Montr.Settings.Models;

namespace Montr.Settings.Queries
{
	public class GetSettingsMetadata : IRequest<ICollection<SettingsBlock>>
	{
		public ClaimsPrincipal Principal { get; set; }

		[Required]
		public string EntityTypeCode { get; set; }

		[Required]
		public Guid EntityUid { get; set; }

		public string Category { get; set; }
	}
}
