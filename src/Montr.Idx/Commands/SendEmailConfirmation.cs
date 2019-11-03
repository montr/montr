using System.ComponentModel.DataAnnotations;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class SendEmailConfirmation : IRequest<ApiResult>
	{
		[Required]
		[EmailAddress]
		[StringLength(128)]
		public string Email { get; set; }
	}
}
