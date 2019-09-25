using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Tendr.Commands
{
	public class SendInvitations: IRequest<ApiResult>
	{
		public Guid EventUid { get; set; }
	}
}
