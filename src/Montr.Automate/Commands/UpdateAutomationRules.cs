using System;
using System.Collections.Generic;
using MediatR;
using Montr.Automate.Models;
using Montr.Core.Models;

namespace Montr.Automate.Commands
{
	public class UpdateAutomationRules : IRequest<ApiResult>
	{
		public Guid AutomationUid { get; set; }

		public IList<AutomationCondition> Conditions { get; set; }

		public IList<AutomationAction> Actions { get; set; }
	}
}
