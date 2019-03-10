using System;
using System.Collections.Generic;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierGroupList : IRequest<ICollection<ClassifierGroup>>
	{
		public Guid UserUid { get; set; }

		public ClassifierGroupSearchRequest Request { get; set; }
	}
}
