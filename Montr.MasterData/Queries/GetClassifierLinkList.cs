using System;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierLinkList : IRequest<SearchResult<ClassifierLink>>
	{
		public Guid UserUid { get; set; }

		public ClassifierLinkSearchRequest Request { get; set; }
	}
}