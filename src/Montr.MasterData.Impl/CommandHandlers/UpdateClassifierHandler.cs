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
	public class UpdateClassifierHandler : IRequestHandler<UpdateClassifier, ApiResult>
	{
		private readonly INamedServiceFactory<IClassifierRepository> _classifierTypeProviderFactory;

		public UpdateClassifierHandler(INamedServiceFactory<IClassifierRepository> classifierTypeProviderFactory)
		{
			_classifierTypeProviderFactory = classifierTypeProviderFactory;
		}

		public async Task<ApiResult> Handle(UpdateClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var classifierTypeProvider = _classifierTypeProviderFactory.GetService(item.Type);

			return await classifierTypeProvider.Update(item, cancellationToken);
		}
	}
}
