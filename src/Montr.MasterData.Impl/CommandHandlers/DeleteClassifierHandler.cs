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
		private readonly INamedServiceFactory<IClassifierRepository> _classifierTypeProviderFactory;

		public DeleteClassifierHandler(INamedServiceFactory<IClassifierRepository> classifierTypeProviderFactory)
		{
			_classifierTypeProviderFactory = classifierTypeProviderFactory;
		}

		public async Task<ApiResult> Handle(DeleteClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var classifierTypeProvider = _classifierTypeProviderFactory.GetNamedOrDefaultService(request.TypeCode);

			return await classifierTypeProvider.Delete(request, cancellationToken);
		}
	}
}
