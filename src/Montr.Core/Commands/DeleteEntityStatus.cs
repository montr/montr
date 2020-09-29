using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Commands
{
	public class DeleteEntityStatus : IRequest<ApiResult>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public string[] Codes { get; set; }
	}
}
