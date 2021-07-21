using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierTypeMetadataHandler : IRequestHandler<GetClassifierTypeMetadata, DataView>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IConfigurationService _configurationService;

		public GetClassifierTypeMetadataHandler(
			IClassifierTypeService classifierTypeService,
			IConfigurationService configurationService)
		{
			_classifierTypeService = classifierTypeService;
			_configurationService = configurationService;
		}

		public async Task<DataView> Handle(GetClassifierTypeMetadata request, CancellationToken cancellationToken)
		{
			var result = new DataView();

			if (request.ViewId == ViewCode.ClassifierTypeTabs)
			{
				var entity = request.TypeCode != null
					? await _classifierTypeService.Get(request.TypeCode, cancellationToken)
					: new ClassifierType(); // for new classifier types

				result.Panes = await _configurationService.GetItems<ClassifierType, DataPane>(entity, request.Principal);
			}

			return result;
		}
	}
}
