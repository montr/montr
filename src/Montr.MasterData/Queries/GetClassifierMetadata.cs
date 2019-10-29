using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierMetadata : IRequest<DataView>
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public string TypeCode { get; set; }
	}
}
