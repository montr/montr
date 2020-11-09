using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Services;
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

		public async Task<IList<FieldMetadata>> GetFields(AutomationContext context, CancellationToken cancellationToken)
		{
			return await _serviceFactory.GetRequiredService(context.EntityTypeCode).GetFields(context, cancellationToken);
		}
	}
}
