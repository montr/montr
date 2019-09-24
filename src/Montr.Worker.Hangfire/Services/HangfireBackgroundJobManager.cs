using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;
using Montr.Worker.Services;

namespace Montr.Worker.Hangfire.Services
{
	public class HangfireBackgroundJobManager : IBackgroundJobManager
	{
		public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
		{
			return BackgroundJob.Enqueue(methodCall);
		}
	}
}
