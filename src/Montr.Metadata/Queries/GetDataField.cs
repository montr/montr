using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Metadata.Queries
{
	public class GetDataField : IRequest<FieldMetadata>
	{
		public Guid UserUid { get; set; }

		public Guid CompanyUid { get; set; }

		public string EntityTypeCode { get; set; }

		public Guid Uid { get; set; }
	}
}
