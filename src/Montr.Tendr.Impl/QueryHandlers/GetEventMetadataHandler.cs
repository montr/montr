using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Services;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Impl.QueryHandlers
{
	public class GetEventMetadataHandler : IRequestHandler<GetEventMetadata, DataView>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IMetadataProvider _metadataProvider;

		public GetEventMetadataHandler(IClassifierTypeService classifierTypeService, IMetadataProvider metadataProvider)
		{
			_classifierTypeService = classifierTypeService;
			_metadataProvider = metadataProvider;
		}

		public async Task<DataView> Handle(GetEventMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView { Id = request.ViewId };

			if (request.ViewId == "Event/Edit")
			{
				result.Fields = new List<FormField>
				{
					new TextAreaField { Key = "name", Name = "Наименование", Rows = 4, Required = true },
					new TextAreaField { Key = "description", Name = "Описание", Rows = 4, Description = "Как можно подробнее опишите что вы хотите купить." }
				};
			}

			return await Task.FromResult(result);
		}
	}
}
