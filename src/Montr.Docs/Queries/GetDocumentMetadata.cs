using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Docs.Queries
{
	public class GetDocumentMetadata: IRequest<DataView>
	{
		public string TypeCode { get; set; }

		public Guid? DocumentTypeUid { get; set; }
	}
}
