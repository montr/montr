using System;
using MediatR;

namespace Montr.MasterData.Commands
{
	public class DeleteClassifierType: IRequest<int>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }
	}
}