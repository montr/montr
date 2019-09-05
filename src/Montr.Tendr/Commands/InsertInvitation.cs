using System;
using MediatR;
using Montr.Metadata.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Commands
{
	public class InsertInvitation : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public Invitation Item { get; set; }
	}
}
