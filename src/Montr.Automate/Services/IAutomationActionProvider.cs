using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;

namespace Montr.Automate.Services
{
	public interface IAutomationActionProvider
	{
		Type ActionType { get; }

		Task Execute(AutomationAction automationAction, AutomationContext context, CancellationToken cancellationToken);
	}
}
