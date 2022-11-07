using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.QueryHandlers
{
	public class GetEventMetadataHandler : IRequestHandler<GetEventMetadata, DataView>
	{
		private readonly IConfigurationProvider _configurationProvider;

		public GetEventMetadataHandler(IConfigurationProvider configurationProvider)
		{
			_configurationProvider = configurationProvider;
		}

		public async Task<DataView> Handle(GetEventMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView { Id = request.ViewId };

			if (request.ViewId == "PrivateEvent/Edit")
			{
				// todo: preload event from service
				var @event = new Event { Uid = request.EventUid };

				result.Panes = await _configurationProvider.GetItems<Event, DataPane>(@event, request.Principal);
			}

			if (request.ViewId == "Event/Edit")
			{
				result.Fields = new List<FieldMetadata>
				{
					new TextAreaField { Key = "name", Name = "Наименование", Required = true, Props = new TextAreaField.Properties { Rows = 4 } },
					new TextAreaField { Key = "description", Name = "Описание", Props = new TextAreaField.Properties { Rows = 4 },
						Description = "Как можно подробнее опишите что вы хотите купить." }
				};
			}

			return await Task.FromResult(result);
		}
	}
}
