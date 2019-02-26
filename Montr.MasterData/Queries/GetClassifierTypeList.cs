using System;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierTypeList : IRequest<DataResult<ClassifierType>>
	{
		public Guid UserUid { get; set; }

		public ClassifierTypeSearchRequest Request { get; set; }
	}
}