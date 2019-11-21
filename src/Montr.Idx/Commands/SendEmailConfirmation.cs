using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class SendEmailConfirmation : IRequest<ApiResult>
	{
		public ClaimsPrincipal User { get; set; }

		/// <summary>
		/// Not required for logged in user
		/// </summary>
		// [Required]
		[EmailAddress]
		[StringLength(128)]
		public string Email { get; set; }
	}
}
