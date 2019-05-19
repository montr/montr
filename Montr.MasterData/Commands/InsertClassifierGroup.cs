using System;
using MediatR;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifierGroup : IRequest<InsertClassifierGroup.Result>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }
		
		public string TreeCode { get; set; }

		public ClassifierGroup Item { get; set; }

		public class Result : ApiResult
		{
			public Guid? Uid { get; set; }
		}
	}
}
