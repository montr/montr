using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Commands
{
	public class InsertEntityStatus : IRequest<ApiResult>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public EntityStatus Item { get; set; }
	}
}
