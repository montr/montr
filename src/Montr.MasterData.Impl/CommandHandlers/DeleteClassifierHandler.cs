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
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		public DeleteClassifierHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_repositoryFactory = repositoryFactory;
		}

		public async Task<ApiResult> Handle(DeleteClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var classifierTypeProvider = _repositoryFactory.GetNamedOrDefaultService(request.TypeCode);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await classifierTypeProvider.Delete(request, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
