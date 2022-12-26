using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Docs.Commands
{
	public class DeleteDocument : IRequest<ApiResult>
	{
		public Guid? UserUid { get; set; }

		public Guid[] Uids { get; set; }

		public string StatusCode { get; set; }
	}
}
