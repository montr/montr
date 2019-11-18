using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class RemoveExternalLogin : IRequest<ApiResult>
	{
		public ClaimsPrincipal User { get; set; }

		[Required]
		[StringLength(36)]
		public string LoginProvider { get; set; }

		[Required]
		[StringLength(128)]
		public string ProviderKey { get; set; }
	}
}
