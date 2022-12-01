using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.Settings.Models;
using Montr.Settings.Queries;
using Montr.Settings.Services.Implementations;

namespace Montr.Settings.Services.QueryHandlers
{
	public class GetSettingsMetadataHandler : IRequestHandler<GetSettingsMetadata, ICollection<SettingsBlock>>
	{
		private readonly INamedServiceFactory<IEntityProvider> _entityProviderFactory;
		private readonly IConfigurationProvider _configurationProvider;
		private readonly ISettingsMetadataProvider _settingsMetadataProvider;

		public GetSettingsMetadataHandler(INamedServiceFactory<IEntityProvider> entityProviderFactory,
			IConfigurationProvider configurationProvider, ISettingsMetadataProvider settingsMetadataProvider)
		{
			_entityProviderFactory = entityProviderFactory;
			_configurationProvider = configurationProvider;
			_settingsMetadataProvider = settingsMetadataProvider;
		}

		public async Task<ICollection<SettingsBlock>> Handle(GetSettingsMetadata request, CancellationToken cancellationToken)
		{
			var result = new List<SettingsBlock>();

			var entityProvider = _entityProviderFactory.GetRequiredService(request.EntityTypeCode);

			var entity = await entityProvider.GetEntity(request.EntityTypeCode, request.EntityUid, cancellationToken);

			var items = await _configurationProvider.GetItems<SettingsPane>(entity.GetType(), entity, request.Principal);

			foreach (var item in items.Where(x => x.Category == request.Category))
			{
				result.Add(new SettingsBlock
				{
					TypeCode = OptionsUtils.GetOptionsSectionKey(item.Type),
					DisplayName = SettingsNameUtils.BuildSettingsName(item.Type.Name),
					Fields = await _settingsMetadataProvider.GetMetadata(item.Type)
				});
			}

			return result;
		}
	}
}
