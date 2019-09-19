using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.MasterData.Commands
{
	public class DeleteClassifierType : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Guid[] Uids { get; set; }
	}
}
