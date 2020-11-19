using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbClassifierRepository : IRepository<Classifier>
	{
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly INamedServiceFactory<IClassifierTypeProvider> _classifierTypeProviderFactory;

		public DbClassifierRepository(
			IClassifierTypeService classifierTypeService,
			INamedServiceFactory<IClassifierTypeProvider> classifierTypeProviderFactory)
		{
			_classifierTypeService = classifierTypeService;
			_classifierTypeProviderFactory = classifierTypeProviderFactory;
		}

		public async Task<SearchResult<Classifier>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			var classifierTypeProvider = _classifierTypeProviderFactory.GetNamedOrDefaultService(type.Code);

			return await classifierTypeProvider.Search(type, request, cancellationToken);
		}
	}
}

