using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Docs.Commands
{
	public class UpdateDocument : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid Uid { get; set; }
	}
}
