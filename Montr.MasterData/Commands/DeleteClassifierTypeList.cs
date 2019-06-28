using System;
using MediatR;

namespace Montr.MasterData.Commands
{
	public class DeleteClassifierTypeList: IRequest<int>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Guid[] Uids { get; set; }
	}
}
