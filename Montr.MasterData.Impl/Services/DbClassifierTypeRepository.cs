using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.Services
{
	public class DbClassifierTypeRepository : IEntityRepository<ClassifierType>
	{
		public async Task<DataResult<ClassifierType>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierTypeSearchRequest)searchRequest;

			var result = new List<ClassifierType>
			{
				new ClassifierType
				{
					IsSystem = true,
					Code = "okved2",
					Name = "ОК видов экономической деятельности (ОКВЭД2)",
					HierarchyType = HierarchyType.Folders
				},
				new ClassifierType
				{
					IsSystem = true,
					Code = "okei",
					Name = "ОК единиц измерения"
				}
			};

			if (request.TypeCode != null)
			{
				result = result.Where(x => x.Code == request.TypeCode).ToList();
			}

			return await Task.FromResult(new DataResult<ClassifierType>
			{
				Rows = result,
				TotalCount = result.Count
			});
		}
	}
}
