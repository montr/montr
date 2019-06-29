using System;
using MediatR;
using Montr.Core.Models;
using Montr.Kompany.Models;

namespace Montr.Kompany.Queries
{
    // todo: remove, use mdm
	public class GetContractorList : IRequest<SearchResult<Company>>
	{
		public Guid UserUid { get; set; }

		public ContractorSearchRequest Request { get; set; }
	}
}
