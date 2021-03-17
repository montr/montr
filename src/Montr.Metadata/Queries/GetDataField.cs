using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Metadata.Queries
{
	public class GetDataField : IRequest<FieldMetadata>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public Guid Uid { get; set; }
	}
}
