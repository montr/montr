using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class RegisterClassifierTypeHandler : IRequestHandler<RegisterClassifierType, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IFieldMetadataService _metadataService;

		public RegisterClassifierTypeHandler(
			IUnitOfWorkFactory unitOfWorkFactory,
			IClassifierTypeService classifierTypeService,
			IFieldMetadataService metadataService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_classifierTypeService = classifierTypeService;
			_metadataService = metadataService;
		}

		public async Task<ApiResult> Handle(RegisterClassifierType request, CancellationToken cancellationToken)
		{
			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var type = await _classifierTypeService.TryGet(item.Code, cancellationToken);

			if (type != null) return new ApiResult { AffectedRows = 0 };

			using (var scope = _unitOfWorkFactory.Create())
			{
				var insertTypeResult = await _classifierTypeService.Insert(item, cancellationToken);

				if (insertTypeResult.Success == false) return insertTypeResult;

				// todo: throw if empty fields
				if (request.Fields != null)
				{
					// ReSharper disable once PossibleInvalidOperationException
					var entityUid = insertTypeResult.Uid.Value;

					foreach (var metadata in request.Fields)
					{
						var insertFieldResult = await _metadataService.Insert(new ManageFieldMetadataRequest
						{
							EntityTypeCode = ClassifierType.EntityTypeCode,
							EntityUid = entityUid,
							Item = metadata
						}, cancellationToken);

						if (insertFieldResult.Success == false) return insertFieldResult;
					}
				}

				scope.Commit();

				return new ApiResult { AffectedRows = 1, Uid = insertTypeResult.Uid };
			}
		}
	}
}
