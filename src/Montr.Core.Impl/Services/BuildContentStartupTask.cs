using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class BuildContentStartupTask : IStartupTask
	{
		private readonly IContentService _contentService;

		public BuildContentStartupTask(IContentService contentService)
		{
			_contentService = contentService;
		}

		public Task Run(CancellationToken cancellationToken)
		{
			_contentService.Rebuild();

			return Task.CompletedTask;
		}
	}
}
