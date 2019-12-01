using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class ExternalLoginCallback : IRequest<ApiResult<ExternalRegisterModel>>
	{
		public string ReturnUrl { get; set; }

		public string RemoteError { get; set; }
	}
}
