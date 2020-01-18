using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Metadata.Commands
{
	public class DeleteDataField : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public Guid[] Uids { get; set; }
	}
}
