using System;
using Kompany.Models;
using MediatR;
using Montr.Metadata.Models;

namespace Kompany.Queries
{
	public class GetContractorList : IRequest<DataResult<Company>>
	{
		public Guid UserUid { get; set; }

		public ContractorSearchRequest Request { get; set; }
	}
}
