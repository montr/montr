using System.ComponentModel.DataAnnotations;
using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class ExternalLoginCallbackCommand : IRequest<ExternalLoginCallbackCommand.Result>
	{
		[Required]
		public string ReturnUrl { get; set; }

		public string RemoteError { get; set; }

		public class Result : ApiResult
		{
			public Result()
			{
			}

			public Result(params string[] errorMessages) : base(errorMessages)
			{
			}
			
			public ExternalRegisterUserModel Register { get; set; }
		}
	}
}
