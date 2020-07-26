using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;
using Montr.Docs.Models;

namespace Montr.Docs.Impl.Services
{
	public class DocumentAutomationContextProvider : IAutomationContextProvider
	{
		private readonly IRepository<Document> _repository;

		public DocumentAutomationContextProvider(IRepository<Document> repository)
		{
			_repository = repository;
		}

		public async Task<object> GetEntity(AutomationContext context, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new DocumentSearchRequest
			{
				Uid = context.EntityUid,
				IncludeFields = true
			}, cancellationToken);

			return result.Rows.Single();
		}
	}
}
