using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Worker.Hangfire.Services;
using NUnit.Framework;

namespace Montr.Worker.Hangfire.Tests.Services
{
	public class HangfireBackgroundJobManagerTests
	{
		[Test]
		public void JobManager_ShouldEnqueueJobs_WithoutTransaction()
		{
			// arrange
			var jobManager = new HangfireBackgroundJobManager();

			// act
			var jobId = jobManager.Enqueue<JobHandler>(x => x.SumJob(1, 2));

			// assert
			Assert.IsNotNull(jobId);
		}

		[Test]
		public void JobManager_ShouldEnqueueJobs_WithTransaction()
		{
			// arrange
			var unitOfWorkFactory = new TransactionScopeUnitOfWorkFactory();
			var jobManager = new HangfireBackgroundJobManager();

			using (var scope = unitOfWorkFactory.Create())
			{
				// act
				var jobId = jobManager.Enqueue<JobHandler>(x => x.SumJob(1, 2));

				// assert
				Assert.IsNotNull(jobId);

				scope.Commit();
			}
		}

		public class JobHandler
		{
			public async Task<int> SumJob(int inc1, int inc2)
			{
				return await Task.FromResult(inc1 + inc2);
			}
		}
	}
}
