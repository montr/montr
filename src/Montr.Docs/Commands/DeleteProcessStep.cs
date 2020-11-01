using System;
using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Docs.Commands
{
	public class DeleteProcessStep : IRequest<ApiResult>
	{
		public Guid ProcessUid { get; set; }

		public IList<Guid> Uids { get; set; }
	}
}
