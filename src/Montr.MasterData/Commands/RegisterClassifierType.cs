using System.Collections.Generic;
using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Commands
{
	// todo: remove
	public class RegisterClassifierType : IRequest<ApiResult>
	{
		public ClassifierType Item { get; set; }

		public ICollection<FieldMetadata> Fields { get; set; }
	}
}
