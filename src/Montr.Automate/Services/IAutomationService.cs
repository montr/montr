using System;
using System.Threading;
using System.Threading.Tasks;

namespace Montr.Automate.Services
{
	public interface IAutomationService
	{
		Task OnChange(string entityTypeCode, Guid entityUid, object entity, CancellationToken cancellationToken);
	}
}
