using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Automate.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Services;

namespace Montr.Automate.Services.Implementations
{
	public class RegisterClassifierTypeStartupTask : IStartupTask
	{
		private readonly IClassifierTypeRegistrator _classifierTypeRegistrator;

		public RegisterClassifierTypeStartupTask(IClassifierTypeRegistrator classifierTypeRegistrator)
		{
			_classifierTypeRegistrator = classifierTypeRegistrator;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			foreach (var classifierType in GetClassifierTypes())
			{
				await _classifierTypeRegistrator.Register(classifierType.Item, classifierType.Fields, cancellationToken);
			}
		}

		protected static IEnumerable<RegisterClassifierType> GetClassifierTypes()
		{
			yield return Automation.GetDefaultMetadata();
		}
	}
}
