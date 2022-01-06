using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Services
{
	/// <summary>
	/// Provider of common information about automation context.
	/// </summary>
	public interface IAutomationContextProvider
	{
		Task<object> GetEntity(AutomationContext context, CancellationToken cancellationToken);

		Task<IList<FieldMetadata>> GetFields(AutomationContext context, CancellationToken cancellationToken);
	}
}
