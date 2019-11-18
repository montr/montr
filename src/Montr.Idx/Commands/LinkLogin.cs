using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Idx.Commands
{
	public class LinkLogin : IRequest<ChallengeResult>
	{
		public ClaimsPrincipal User { get; set; }

		public string ReturnUrl { get; set; }

		[Required]
		[StringLength(128)]
		public string Provider { get; set; }
	}
}
