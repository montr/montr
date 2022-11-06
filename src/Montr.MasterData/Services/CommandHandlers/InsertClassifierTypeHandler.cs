using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Services.CommandHandlers
{
	public class InsertClassifierTypeHandler : IRequestHandler<InsertClassifierType, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IFieldMetadataService _metadataService;

		public InsertClassifierTypeHandler(IUnitOfWorkFactory unitOfWorkFactory,
			IClassifierTypeService classifierTypeService, IFieldMetadataService metadataService)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_classifierTypeService = classifierTypeService;
			_metadataService = metadataService;
		}

		public async Task<ApiResult> Handle(InsertClassifierType request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				var insertTypeResult = await _classifierTypeService.Insert(item, cancellationToken);

				if (insertTypeResult.Success == false) return insertTypeResult;

				var insertFieldResult = await _metadataService.Insert(new ManageFieldMetadataRequest
				{
					EntityTypeCode = ClassifierType.TypeCode,
					// ReSharper disable once PossibleInvalidOperationException
					EntityUid = insertTypeResult.Uid.Value,
					Items = ClassifierMetadata.GetDefaultFields()
				}, cancellationToken);

				if (insertFieldResult.Success) scope.Commit();

				return insertTypeResult;
			}
		}
	}
}
