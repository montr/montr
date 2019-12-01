using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class ExternalLoginCallback : IRequest<ExternalLoginCallback.Result>
	{
		public string ReturnUrl { get; set; }

		public string RemoteError { get; set; }

		// todo: create ApiResult<TModel>
		public class Result : ApiResult
		{
			public ExternalRegisterModel Register { get; set; }
		}
	}
}
