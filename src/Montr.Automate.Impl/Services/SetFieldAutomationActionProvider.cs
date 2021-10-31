using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Impl.Models;
using Montr.Automate.Models;
using Montr.Automate.Services;
using Montr.Metadata.Models;

namespace Montr.Automate.Impl.Services
{
	public class SetFieldAutomationActionProvider : IAutomationActionProvider
	{
		public AutomationRuleType RuleType => new()
		{
			Code = SetFieldAutomationAction.TypeCode,
			Name = "Set Field",
			Type = typeof(SetFieldAutomationAction)
		};

		public IList<FieldMetadata> GetMetadata()
		{
			throw new System.NotImplementedException();
		}

		public Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}
