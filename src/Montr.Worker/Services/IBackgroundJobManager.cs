using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Montr.Worker.Services
{
	public interface IBackgroundJobManager
	{
		string Enqueue<T>(Expression<Func<T, Task>> methodCall);
	}
}
