using System;
using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Commands
{
	public class DeleteEntityStatus : IRequest<ApiResult>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public IList<Guid> Uids { get; set; }
	}
}
