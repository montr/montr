using System;
using MediatR;

namespace Montr.MasterData.Commands
{
	public class DeleteClassifierList: IRequest<int>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		public Guid[] Uids { get; set; }
	}
}
