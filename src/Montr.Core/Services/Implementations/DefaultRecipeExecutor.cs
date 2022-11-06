using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services.Implementations
{
	public class DefaultRecipeExecutor : IRecipeExecutor
	{
		public Task<string> Execute(RecipeDescriptor descriptor, CancellationToken cancellationToken = default)
		{
			throw new System.NotImplementedException();
		}
	}
}
