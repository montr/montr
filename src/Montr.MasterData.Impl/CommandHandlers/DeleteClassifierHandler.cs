using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class DeleteClassifierHandler : IRequestHandler<DeleteClassifier, ApiResult>
	{
		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		public DeleteClassifierHandler(INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_repositoryFactory = repositoryFactory;
		}

		public async Task<ApiResult> Handle(DeleteClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var classifierTypeProvider = _repositoryFactory.GetNamedOrDefaultService(request.TypeCode);

			return await classifierTypeProvider.Delete(request, cancellationToken);
		}
	}
}
