using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IRecipeExecutor
	{
		Task<string> Execute(RecipeDescriptor descriptor, CancellationToken cancellationToken = default);
	}
}
