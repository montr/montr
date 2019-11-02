using System.ComponentModel.DataAnnotations;
using MediatR;
using Montr.Core.Models;

namespace Montr.Idx.Commands
{
	public class ExternalLoginCallbackCommand : IRequest<ApiResult>
	{
		[Required]
		public string ReturnUrl { get; set; }

		public string RemoteError { get; set; }
	}
}
