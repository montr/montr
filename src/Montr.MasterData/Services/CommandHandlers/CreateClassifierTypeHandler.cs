using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services.CommandHandlers
{
	public class CreateClassifierTypeHandler : IRequestHandler<CreateClassifierType, ClassifierType>
	{
		public Task<ClassifierType> Handle(CreateClassifierType request, CancellationToken cancellationToken)
		{
			var result = new ClassifierType { HierarchyType = HierarchyType.None };

			return Task.FromResult(result);
		}
	}
}
