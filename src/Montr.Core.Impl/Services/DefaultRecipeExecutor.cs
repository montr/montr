using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultRecipeExecutor : IRecipeExecutor
	{
		public Task<string> Execute(RecipeDescriptor descriptor, CancellationToken cancellationToken = default)
		{
			throw new System.NotImplementedException();
		}
	}
}
