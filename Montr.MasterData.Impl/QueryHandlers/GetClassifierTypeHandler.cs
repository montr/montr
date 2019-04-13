using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierTypeHandler : IRequestHandler<GetClassifierType, ClassifierType>
	{
		private readonly IClassifierTypeService _classifierTypeService;

		public GetClassifierTypeHandler(IClassifierTypeService classifierTypeService)
		{
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ClassifierType> Handle(GetClassifierType command, CancellationToken cancellationToken)
		{
			return await _classifierTypeService.GetClassifierType(command.CompanyUid, command.TypeCode, cancellationToken);
		}
	}
}
