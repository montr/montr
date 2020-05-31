using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Automate.Impl.Services
{
	public class DbAutomationRepository : IRepository<Automation>
	{
		public Task<SearchResult<Automation>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (AutomationSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			var result = new SearchResult<Automation>();

			if (request.EntityTypeCode == "DocumentType" &&
				request.EntityUid == Guid.Parse("ab770d9f-f723-4468-8807-5df0f6637cca"))
			{
				var automation = new Automation
				{
					IsActive = true,
					Conditions = new List<AutomationCondition>
					{
						new FieldAutomationCondition
						{
							Meet = AutomationConditionMeet.All,
							Field = "StatusCode",
							Operator = AutomationConditionOperator.Equal,
							Value = "Published"
						}
					},
					Actions = new List<AutomationAction>
					{
						new NotifyByEmailAutomationAction
						{
							Recipient = "operator",
							Subject = "New company registration request published",
							Body = "New company registration request {{DocumentNumber}} published, please review."
						},
						new NotifyByEmailAutomationAction
						{
							Recipient = "requester",
							Subject = "Your company registration request received",
							Body = "Your company registration request {{DocumentNumber}} received and will be reviewed."
						}
					}
				};

				result.Rows = new[] { automation };
			}

			return Task.FromResult(result);
		}
	}
}
