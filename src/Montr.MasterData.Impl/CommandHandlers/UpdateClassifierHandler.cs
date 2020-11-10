using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class UpdateClassifierHandler : IRequestHandler<UpdateClassifier, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IClassifierTreeService _classifierTreeService;
		private readonly INamedServiceFactory<IClassifierTypeProvider> _classifierTypeProviderFactory;
		private readonly IClassifierTypeMetadataService _metadataService;
		private readonly IFieldDataRepository _fieldDataRepository;

		public UpdateClassifierHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IClassifierTreeService classifierTreeService,
			INamedServiceFactory<IClassifierTypeProvider> classifierTypeProviderFactory,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
			_classifierTreeService = classifierTreeService;
			_classifierTypeProviderFactory = classifierTypeProviderFactory;
			_metadataService = metadataService;
			_fieldDataRepository = fieldDataRepository;
		}

		public async Task<ApiResult> Handle(UpdateClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			var tree = type.HierarchyType == HierarchyType.Groups
				? await _classifierTreeService.GetClassifierTree(request.CompanyUid, request.TypeCode, ClassifierTree.DefaultCode, cancellationToken)
				: null;

			// todo: validate fields
			var metadata = await _metadataService.GetMetadata(type, cancellationToken);

			var manageFieldDataRequest = new ManageFieldDataRequest
			{
				EntityTypeCode = Classifier.TypeCode,
				// ReSharper disable once PossibleInvalidOperationException
				EntityUid = item.Uid.Value,
				Metadata = metadata,
				Item = item
			};

			// todo: move to ClassifierValidator (?)
			var result = await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);

			if (result.Success == false)
			{
				return result;
			}

			var classifierTypeProvider = _classifierTypeProviderFactory.GetService(type.Code);

			using (var scope = _unitOfWorkFactory.Create())
			{
				int affected;

				using (var db = _dbContextFactory.Create())
				{
					var validator = new ClassifierValidator(db, type);

					if (await validator.ValidateUpdate(item, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = validator.Errors };
					}

					affected = await db.GetTable<DbClassifier>()
						.Where(x => x.Uid == item.Uid)
						.Set(x => x.Code, item.Code)
						.Set(x => x.Name, item.Name)
						.Set(x => x.ParentUid, type.HierarchyType == HierarchyType.Items ? item.ParentUid : null)
						.UpdateAsync(cancellationToken);

					if (type.HierarchyType == HierarchyType.Groups)
					{
						// todo: combine with InsertClassifierLinkHandler in one service

						// delete other links in same tree
						var deleted = await (
							from link in db.GetTable<DbClassifierLink>().Where(x => x.ItemUid == item.Uid)
							join groups in db.GetTable<DbClassifierGroup>() on link.GroupUid equals groups.Uid
							where groups.TreeUid == tree.Uid
							select link
						).DeleteAsync(cancellationToken);

                        // todo: check parent belongs to default tree
						if (item.ParentUid != null)
						{
							var inserted = await db.GetTable<DbClassifierLink>()
								.Value(x => x.GroupUid, item.ParentUid)
								.Value(x => x.ItemUid, item.Uid)
								.InsertAsync(cancellationToken);
						}
					}
					else if (type.HierarchyType == HierarchyType.Items)
					{
						var closureTable = new ClosureTableHandler(db, type);

						// ReSharper disable once PossibleInvalidOperationException
						if (await closureTable.Update(item.Uid.Value, item.ParentUid, cancellationToken) == false)
						{
							return new ApiResult { Success = false, Errors = closureTable.Errors };
						}
					}
				}

				// update fields
				await _fieldDataRepository.Update(manageFieldDataRequest, cancellationToken);

				// update specific classifier table
				if (classifierTypeProvider != null) await classifierTypeProvider.Update(type, item, cancellationToken);

				// todo: (события)

				scope.Commit();

				return new ApiResult { AffectedRows = affected };
			}
		}
	}
}
