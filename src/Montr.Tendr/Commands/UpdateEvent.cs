using System;
using MediatR;
using Montr.Metadata.Models;
using Montr.Tendr.Models;

namespace Montr.Tendr.Commands
{
	public class UpdateEvent : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Event Item { get; set; }
	}
}
