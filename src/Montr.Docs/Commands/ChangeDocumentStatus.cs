using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Docs.Commands
{
	public class ChangeDocumentStatus : IRequest<ApiResult>
	{
		public Guid? DocumentUid { get; set; }

		public string StatusCode { get; set; }
	}
}
