using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Docs.Commands;
using Montr.Docs.Services;

namespace Montr.Docs.Impl.CommandHandlers
{
	public class RegisterDocumentTypeHandler : IRequestHandler<RegisterDocumentType, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDocumentTypeService _documentTypeService;

		public RegisterDocumentTypeHandler(IUnitOfWorkFactory unitOfWorkFactory, IDocumentTypeService documentTypeService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_documentTypeService = documentTypeService;
		}

		// todo: register numerator
		public async Task<ApiResult> Handle(RegisterDocumentType request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var type = await _documentTypeService.TryGet(item.Code, cancellationToken);

			if (type != null) return new ApiResult { AffectedRows = 0 };

			using (var scope = _unitOfWorkFactory.Create())
			{
				var insertTypeResult = await _documentTypeService.Insert(item, cancellationToken);

				if (insertTypeResult.Success == false) return insertTypeResult;

				scope.Commit();

				return new ApiResult { AffectedRows = 1, Uid = insertTypeResult.Uid };
			}
		}
	}
}
