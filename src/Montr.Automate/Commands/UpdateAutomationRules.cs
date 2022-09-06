using MediatR;
using Montr.Automate.Models;
using Montr.Core.Models;

namespace Montr.Automate.Commands
{
	public class UpdateAutomationRules : IRequest<ApiResult>
	{
		public Automation Item { get; set; }
	}
}
