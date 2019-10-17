using System;
using MediatR;
using Montr.Core.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Commands
{
	public class UpdateInvitation : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Guid EventUid { get; set; }

		public Invitation Item { get; set; }
	}
}
