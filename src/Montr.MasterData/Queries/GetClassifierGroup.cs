using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Queries
{
	public class GetClassifierGroup : IRequest<ClassifierGroup>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		public Guid TreeUid { get; set; }

		public Guid Uid { get; set; }
	}
}
