using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Tendr.Models;
using Montr.Tendr.Queries;

namespace Montr.Tendr.Impl.QueryHandlers
{
	public class GetEventMetadataHandler : IRequestHandler<GetEventMetadata, DataView>
	{
		private readonly IConfigurationManager _configurationManager;

		public GetEventMetadataHandler(IConfigurationManager configurationManager)
		{
			_configurationManager = configurationManager;
		}

		public async Task<DataView> Handle(GetEventMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView { Id = request.ViewId };

			if (request.ViewId == "PrivateEvent/Edit")
			{
				// todo: preload event from service
				var @event = new Event { Uid = request.EventUid };

				// todo: authorize before sort
				var items = _configurationManager.GetItems<Event, DataPane>(@event);

				result.Panes = items.OrderBy(x => x.DisplayOrder).ToImmutableList();
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
