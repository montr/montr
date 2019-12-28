using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class GetFieldTypes : IRequest<IList<FieldType>>
	{
		public string EntityTypeCode { get; set; }
	}
}
