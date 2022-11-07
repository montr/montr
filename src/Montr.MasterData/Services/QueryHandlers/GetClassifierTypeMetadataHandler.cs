using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.Metadata.Models;

namespace Montr.MasterData.Services.QueryHandlers
{
	public class GetClassifierTypeMetadataHandler : IRequestHandler<GetClassifierTypeMetadata, DataView>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IConfigurationProvider _configurationProvider;

		public GetClassifierTypeMetadataHandler(
			IClassifierTypeService classifierTypeService,
			IConfigurationProvider configurationProvider)
		{
			_classifierTypeService = classifierTypeService;
			_configurationProvider = configurationProvider;
		}

		public async Task<DataView> Handle(GetClassifierTypeMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView();

			if (request.ViewId == ViewCode.ClassifierTypeTabs)
			{
				var entity = request.TypeCode != null
					? await _classifierTypeService.Get(request.TypeCode, cancellationToken)
					: new ClassifierType(); // for new classifier types

				result.Panes = await _configurationProvider.GetItems<ClassifierType, DataPane>(entity, request.Principal);
			}

			return result;
		}
	}
}
