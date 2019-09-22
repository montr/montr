using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Tendr.Commands
{
	public class PublishEvent: IRequest<ApiResult>
	{
		public Guid CompanyUid { get; set; }

		public Guid UserUid { get; set; }

		public long EventId { get; set; }
	}
}
