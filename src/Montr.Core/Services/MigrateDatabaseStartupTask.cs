using System.Threading;
using System.Threading.Tasks;

namespace Montr.Core.Services
{
	public class MigrateDatabaseStartupTask : IStartupTask
	{
		private readonly IMigrationRunner _migrationRunner;

		public MigrateDatabaseStartupTask(IMigrationRunner migrationRunner)
		{
			_migrationRunner = migrationRunner;
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			await _migrationRunner.Run(cancellationToken);
		}
	}
}
