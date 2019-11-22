using System.ComponentModel.DataAnnotations;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class ConfirmEmailChange : IRequest<ApiResult>
	{
		[Required]
		[StringLength(36, MinimumLength = 36)]
		public string UserId { get; set; }

		[Required]
		[StringLength(128)]
		public string Email { get; set; }

		[Required]
		public string Code { get; set; }
	}
}
