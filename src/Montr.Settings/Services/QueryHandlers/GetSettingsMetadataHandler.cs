using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Settings.Models;
using Montr.Settings.Queries;

namespace Montr.Settings.Services.QueryHandlers
{
	public class GetSettingsMetadataHandler : IRequestHandler<GetSettingsMetadata, ICollection<SettingsBlock>>
	{
		private readonly IConfigurationProvider _configurationProvider;
		private readonly ISettingsTypeRegistry _settingsTypeRegistry;
		private readonly ISettingsMetadataProvider _settingsMetadataProvider;

		public GetSettingsMetadataHandler(IConfigurationProvider configurationProvider,
			ISettingsTypeRegistry settingsTypeRegistry, ISettingsMetadataProvider settingsMetadataProvider)
		{
			_configurationProvider = configurationProvider;
			_settingsTypeRegistry = settingsTypeRegistry;
			_settingsMetadataProvider = settingsMetadataProvider;
		}

		public async Task<ICollection<SettingsBlock>> Handle(GetSettingsMetadata request, CancellationToken cancellationToken)
		{
			var result = new List<SettingsBlock>();

			var items = await _configurationProvider
				.GetItems<Application, SettingsPane>(new Application(), request.Principal);

			foreach (var item in items)
			{
				var metadata = await _settingsMetadataProvider.GetMetadata(item.Type);

				result.Add(new SettingsBlock
				{
					TypeCode = _settingsTypeRegistry.GetTypeCode(item.Type),
					DisplayName = item.Type.Name,
					Fields = metadata
				});
			}

			return result;
		}
	}
}
