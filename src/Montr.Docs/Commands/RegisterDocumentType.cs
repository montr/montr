using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;
using Montr.Docs.Models;
using Montr.Metadata.Models;

namespace Montr.Docs.Commands
{
	public class RegisterDocumentType : IRequest<ApiResult>
	{
		public DocumentType Item { get; set; }

		public ICollection<FieldMetadata> Fields { get; set; }
	}
}
