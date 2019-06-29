using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.MasterData.Commands
{
	public class DeleteClassifierGroup: IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string TypeCode { get; set; }

		public Guid Uid { get; set; }
	}
}
