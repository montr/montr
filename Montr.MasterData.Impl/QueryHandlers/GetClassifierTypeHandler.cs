using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierTypeHandler : IRequestHandler<GetClassifierType, ClassifierType>
	{
		private readonly IRepository<ClassifierType> _repository;

		public GetClassifierTypeHandler(IRepository<ClassifierType> repository)
		{
			_repository = repository;
		}

		public async Task<ClassifierType> Handle(GetClassifierType command, CancellationToken cancellationToken)
		{
			var request = new ClassifierTypeSearchRequest
			{
				UserUid = command.UserUid,
				CompanyUid = command.CompanyUid,
				Code = command.TypeCode
			};

			var types = await _repository.Search(request, cancellationToken);

			return types.Rows.Single();
		}
	}
}