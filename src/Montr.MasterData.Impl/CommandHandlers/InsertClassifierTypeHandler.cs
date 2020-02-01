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
	public class InsertClassifierTypeHandler : IRequestHandler<InsertClassifierType, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public InsertClassifierTypeHandler(IUnitOfWorkFactory unitOfWorkFactory,
			IClassifierTypeService classifierTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ApiResult> Handle(InsertClassifierType request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var result = await _classifierTypeService.Insert(item, cancellationToken);

				if (result.Success) scope.Commit();

				return result;
			}
		}
	}
}
