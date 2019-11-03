using System.ComponentModel.DataAnnotations;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class ForgotPassword : IRequest<ApiResult>
	{
		[Required]
		[EmailAddress]
		[StringLength(128)]
		public string Email { get; set; }
	}
}
