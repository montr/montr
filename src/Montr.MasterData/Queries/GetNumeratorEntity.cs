using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetNumeratorEntity : IRequest<NumeratorEntity>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }
	}
}
