using System;
using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Metadata.Queries
{
	public class GetFieldTypes : IRequest<IList<FieldType>>
	{
		public Guid UserUid { get; set; }

		public string EntityTypeCode { get; set; }
	}
}
