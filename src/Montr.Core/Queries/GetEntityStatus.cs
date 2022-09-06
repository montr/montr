using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class GetEntityStatus : IRequest<EntityStatus>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public Guid Uid { get; set; }
	}
}
