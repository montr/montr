using System;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierTypeList : IRequest<SearchResult<ClassifierType>>
	{
		public ClassifierTypeSearchRequest Request { get; set; }
	}
}
