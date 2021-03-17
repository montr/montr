using System.Collections.Generic;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Metadata.Queries
{
	public class GetFieldTypes : IRequest<IList<FieldType>>
	{
		public string EntityTypeCode { get; set; }
	}
}
