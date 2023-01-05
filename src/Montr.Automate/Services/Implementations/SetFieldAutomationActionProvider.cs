using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Models;
using Montr.Metadata.Models;

namespace Montr.Automate.Services.Implementations
{
	public class SetFieldAutomationActionProvider : IAutomationActionProvider
	{
		public AutomationRuleType RuleType => new()
		{
			Code = SetFieldAutomationAction.TypeCode,
			Name = "Set Field",
			Type = typeof(SetFieldAutomationAction)
		};

		public Task<IList<FieldMetadata>> GetMetadata(
			AutomationContext context, AutomationAction action, CancellationToken cancellationToken = default)
		{
			throw new System.NotImplementedException();
		}

		public Task<ApiResult> Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}
