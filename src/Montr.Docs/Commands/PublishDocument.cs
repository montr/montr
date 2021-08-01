using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Docs.Commands
{
	public class PublishDocument : IRequest<ApiResult>
	{
		public Guid? DocumentUid { get; set; }
	}
}
