using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Montr.Idx.Commands
{
	public class ExternalLoginCommand : IRequest<ChallengeResult>
	{
		// todo: remove, use client redirect?
		public string ReturnUrl { get; set; }

		[Required]
		[StringLength(128)]
		public string Provider { get; set; }
	}

	public class ExternalLoginCallbackCommand : IRequest<IActionResult>
	{
		// todo: remove, use client redirect?
		public string ReturnUrl { get; set; }

		public string RemoteError { get; set; }

		[Required]
		[StringLength(128)]
		public string LoginProvider { get; set; }
	}
}
