using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifierGroup : IRequest<Guid>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }
		
		public string TreeCode { get; set; }

		public ClassifierGroup Item { get; set; }
	}
}