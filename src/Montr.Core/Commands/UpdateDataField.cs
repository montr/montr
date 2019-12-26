using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Commands
{
	public class UpdateDataField : IRequest<ApiResult>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string EntityTypeCode { get; set; }

		public DataField Item { get; set; }
	}
}
