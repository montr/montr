using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;

namespace Montr.MasterData.Services.CommandHandlers
{
	public class UpdateClassifierHandler : IRequestHandler<UpdateClassifier, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly INamedServiceFactory<IClassifierRepository> _repositoryFactory;

		public UpdateClassifierHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			INamedServiceFactory<IClassifierRepository> repositoryFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_repositoryFactory = repositoryFactory;
		}

		public async Task<ApiResult> Handle(UpdateClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var classifierTypeProvider = _repositoryFactory.GetNamedOrDefaultService(item.Type);

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await classifierTypeProvider.Update(item, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
