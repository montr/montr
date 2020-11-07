using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DefaultClassifierTypeRegistrator : IClassifierTypeRegistrator
	{
		private readonly ILogger<DefaultClassifierTypeRegistrator> _logger;
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IFieldMetadataService _metadataService;

		public DefaultClassifierTypeRegistrator(
			ILogger<DefaultClassifierTypeRegistrator> logger,
			IUnitOfWorkFactory unitOfWorkFactory,
			IClassifierTypeService classifierTypeService,
			IFieldMetadataService metadataService)
		{
			_logger = logger;
			_unitOfWorkFactory = unitOfWorkFactory;
			_classifierTypeService = classifierTypeService;
			_metadataService = metadataService;
		}

		public async Task<ApiResult> Register(ClassifierType item, ICollection<FieldMetadata> fields, CancellationToken cancellationToken)
		{
			var type = await _classifierTypeService.TryGet(item.Code, cancellationToken);

			// todo: compare fields of registered classifier and fields from parameters to update them in db
			if (type != null)
			{
				_logger.LogDebug("Classifier type {code} already registered.", item.Code);

				return new ApiResult { AffectedRows = 0 };
			}

			// todo: register numerator

			using (var scope = _unitOfWorkFactory.Create())
			{
				var insertTypeResult = await _classifierTypeService.Insert(item, cancellationToken);

				insertTypeResult.AssertSuccess(() => $"Failed to register classifier type \"{item.Code}\"");

				// todo: throw if empty fields or get default fields?
				if (fields != null)
				{
					// ReSharper disable once PossibleInvalidOperationException
					var entityUid = insertTypeResult.Uid.Value;

					foreach (var metadata in fields)
					{
						var insertFieldResult = await _metadataService.Insert(new ManageFieldMetadataRequest
						{
							EntityTypeCode = ClassifierType.EntityTypeCode,
							EntityUid = entityUid,
							Item = metadata
						}, cancellationToken);

						insertFieldResult.AssertSuccess(() => $"Failed to register classifier type \"{item.Code}\"");
					}
				}

				scope.Commit();

				_logger.LogInformation("Classifier type {code} successfully registered.", item.Code);

				return new ApiResult { AffectedRows = 1, Uid = insertTypeResult.Uid };
			}
		}
	}
}
