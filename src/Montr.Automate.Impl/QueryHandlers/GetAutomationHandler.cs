using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Automate.Models;
using Montr.Automate.Queries;
using Montr.Core.Services;

namespace Montr.Automate.Impl.QueryHandlers
{
	public class GetAutomationHandler : IRequestHandler<GetAutomation, Automation>
	{
		private readonly IRepository<Automation> _repository;

		public GetAutomationHandler(IRepository<Automation> repository)
		{
			_repository = repository;
		}

		public async Task<Automation> Handle(GetAutomation request, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new AutomationSearchRequest
			{
				EntityTypeCode = request.EntityTypeCode,
				EntityUid = request.EntityUid,
				Uid = request.Uid,
				IncludeRules = true
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
