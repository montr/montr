using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierMetadata : IRequest<DataView>
	{
		public Guid UserUid { get; set; }

		public string TypeCode { get; set; }
	}
}
