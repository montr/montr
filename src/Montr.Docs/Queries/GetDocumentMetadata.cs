using System;
using MediatR;
using Montr.Metadata.Models;

namespace Montr.Docs.Queries
{
	public class GetDocumentMetadata: IRequest<DataView>
	{
		public Guid? DocumentTypeUid { get; set; }
	}
}
