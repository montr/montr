using System;
using MediatR;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Metadata.Commands
{
	public class UpdateDataField : IRequest<ApiResult>
	{
		public string EntityTypeCode { get; set; }

		public Guid EntityUid { get; set; }

		public FieldMetadata Item { get; set; }
	}
}
