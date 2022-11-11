using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Metadata.Models;
using Montr.Settings.Queries;

namespace Montr.Settings.Services.QueryHandlers
{
	public class GetSettingsMetadataHandler : IRequestHandler<GetSettingsMetadata, DataView>
	{
		private readonly IConfigurationProvider _configurationProvider;
		private readonly ISettingsMetadataProvider _settingsMetadataProvider;

		public GetSettingsMetadataHandler(
			IConfigurationProvider configurationProvider, ISettingsMetadataProvider settingsMetadataProvider)
		{
			_configurationProvider = configurationProvider;
			_settingsMetadataProvider = settingsMetadataProvider;
		}

		public async Task<DataView> Handle(GetSettingsMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView();

			var items = await _configurationProvider
				.GetItems<Application, SettingsPane>(new Application(), request.Principal);

			foreach (var item in items)
			{
				var metadata = await _settingsMetadataProvider.GetMetadata(item.OptionsType);

				result.Fields = metadata;
			}

			return result;
		}
	}
}
