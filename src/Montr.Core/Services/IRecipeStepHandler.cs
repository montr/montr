using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IRecipeStepHandler
	{
		Task Handle(RecipeExecutionContext context, CancellationToken cancellationToken = default);
	}
}
