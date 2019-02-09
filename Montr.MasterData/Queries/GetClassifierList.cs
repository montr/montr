using System;
using MediatR;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierList : IRequest<DataResult<Classifier>>
	{
		public Guid UserUid { get; set; }

		public ClassifierSearchRequest Request { get; set; }
	}
}
