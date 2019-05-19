using System;
using MediatR;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifierType : IRequest<InsertClassifierType.Result>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public ClassifierType Item { get; set; }

		public class Result : ApiResult
		{
			public Guid? Uid { get; set; }
		}
	}
}
