using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class CreateClassifierHandler : IRequestHandler<CreateClassifier, Classifier>
	{
		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		public CreateClassifierHandler(INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_repositoryFactory = repositoryFactory;
		}

		public async Task<Classifier> Handle(CreateClassifier request, CancellationToken cancellationToken)
		{
			var repository = _repositoryFactory.GetNamedOrDefaultService(request.TypeCode);

			return await repository.Create(request, cancellationToken);
		}
	}
}
