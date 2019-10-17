using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Tendr.Commands
{
	public class DeleteInvitation : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Guid[] Uids { get; set; }
	}
}
