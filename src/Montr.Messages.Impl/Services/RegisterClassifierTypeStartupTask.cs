using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Services;
using Montr.Messages.Models;

namespace Montr.Messages.Impl.Services
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
			cancellationToken.ThrowIfCancellationRequested();

			foreach (var command in GetCommands())
			{
				await _classifierTypeRegistrator.Register(command.Item, command.Fields, cancellationToken);
			}
		}

		protected static IEnumerable<RegisterClassifierType> GetCommands()
		{
			yield return MessageTemplate.GetDefaultMetadata();
		}
	}
}
