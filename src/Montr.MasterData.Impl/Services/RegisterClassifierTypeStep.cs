using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Core.Services;

namespace Montr.MasterData.Impl.Services
{
	public class RegisterClassifierTypeStep : IRecipeStepHandler
	{
		public static readonly string Name = "RegisterClassifierType";

		public Task Handle(RecipeExecutionContext context, CancellationToken cancellationToken = default)
		{
			throw new System.NotImplementedException();
		}
	}
}
