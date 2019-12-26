using System;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class GetDataField : IRequest<DataField>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid Uid { get; set; }
	}
}
