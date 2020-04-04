using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetNumerator : IRequest<Numerator>
	{
		public Guid Uid { get; set; }
	}
}
