using System;
using MediatR;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class InsertClassifierType : IRequest<Guid>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public ClassifierType Item { get; set; }
	}
}