using System;
using MediatR;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Docs.Commands
{
	public class UpdateDocumentForm : IFieldDataContainer, IRequest<ApiResult>
	{
		public Guid DocumentUid { get; set; }

		public FieldData Fields { get; set; }
	}
}