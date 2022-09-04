using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Core.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Impl.Services;

public class DefaultAutomationContextProvider : IAutomationContextProvider
{
	private static readonly string EntityKey = $"{typeof(IAutomationContextProvider)}/EntityKey";

	private readonly INamedServiceFactory<IAutomationContextProvider> _serviceFactory;

	public DefaultAutomationContextProvider(INamedServiceFactory<IAutomationContextProvider> serviceFactory)
	{
		_serviceFactory = serviceFactory;
	}

	public async Task<object> GetEntity(AutomationContext context, CancellationToken cancellationToken)
	{
		if (context.Values.TryGetValue(EntityKey, out var entity) == false)
		{
			var contextProvider = _serviceFactory.GetRequiredService(context.EntityTypeCode);

			entity = context.Values[EntityKey] = await contextProvider.GetEntity(context, cancellationToken);
		}

		return entity;
	}

	// todo: change AutomationContext to something other - GetFields should be used in automation design time,
	// and AutomationContext is not accessible yet. Maybe use Automation or automation uid
	public async Task<IList<FieldMetadata>> GetFields(AutomationContext context, CancellationToken cancellationToken)
	{
		var contextProvider = _serviceFactory.GetRequiredService(context.EntityTypeCode);

		return await contextProvider.GetFields(context, cancellationToken);
	}
}
