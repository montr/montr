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
	public class InsertClassifierHandler : IRequestHandler<InsertClassifier, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IDateTimeProvider _dateTimeProvider;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IRepository<FieldMetadata> _fieldMetadataRepository;
		private readonly IFieldDataRepository _fieldDataRepository;

		public InsertClassifierHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory,
			IDateTimeProvider dateTimeProvider, IClassifierTypeService classifierTypeService,
			IRepository<FieldMetadata> fieldMetadataRepository, IFieldDataRepository fieldDataRepository)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_dateTimeProvider = dateTimeProvider;
			_classifierTypeService = classifierTypeService;
			_fieldMetadataRepository = fieldMetadataRepository;
			_fieldDataRepository = fieldDataRepository;
		}

		public async Task<ApiResult> Handle(InsertClassifier request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			var now = _dateTimeProvider.GetUtcNow();

			// todo: check company belongs to user
			var type = await _classifierTypeService.GetClassifierType(request.CompanyUid, request.TypeCode, cancellationToken);

			var itemUid = Guid.NewGuid();

			// validate fields
			var metadata = await _fieldMetadataRepository.Search(new MetadataSearchRequest
			{
				EntityTypeCode = Classifier.EntityTypeCode + "." + type.Code,
				IsSystem = false,
				IsActive = true
			}, cancellationToken);

			var manageFieldDataRequest = new ManageFieldDataRequest
			{
				EntityTypeCode = Classifier.EntityTypeCode,
				EntityUid = itemUid,
				Metadata = metadata.Rows,
				Item = item
			};

			// todo: move to ClassifierValidator (?)
			var result = await _fieldDataRepository.Validate(manageFieldDataRequest, cancellationToken);

			if (result.Success == false)
			{
				return result;
			}

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					var validator = new ClassifierValidator(db, type);

					if (await validator.ValidateInsert(item, cancellationToken) == false)
					{
						return new ApiResult { Success = false, Errors = validator.Errors };
					}

					// todo: company + modification data

					// insert classifier
					await db.GetTable<DbClassifier>()
						.Value(x => x.Uid, itemUid)
						// .Value(x => x.CompanyUid, request.CompanyUid)
						.Value(x => x.TypeUid, type.Uid)
						.Value(x => x.StatusCode, ClassifierStatusCode.Active)
						.Value(x => x.Code, item.Code)
						.Value(x => x.Name, item.Name)
						// todo: validate parent belongs to the same classifier
						.Value(x => x.ParentUid, type.HierarchyType == HierarchyType.Items ? item.ParentUid : null)
						.InsertAsync(cancellationToken);


					if (type.HierarchyType == HierarchyType.Groups)
					{
						// todo: validate group belongs to the same classifier
						// todo: validate selected group belong to default tree

						// what if insert classifier and no groups inserted before?
						/*if (request.TreeUid == null || request.ParentUid == null)
						{
							throw new InvalidOperationException("Classifier should belong to one of the default hierarchy group.");
						}*/

						if (/*request.TreeUid != null &&*/ item.ParentUid != null)
						{
							// link to selected group
							await db.GetTable<DbClassifierLink>()
								.Value(x => x.GroupUid, item.ParentUid)
								.Value(x => x.ItemUid, itemUid)
								.InsertAsync(cancellationToken);
						}

						// if group is not of default hierarchy, link to default hierarchy root
						/*var root = await GetRoot(db, request.ParentUid.Value, cancellationToken);

						if (root.Code != ClassifierTree.DefaultCode)
						{
							await LinkToDefaultRoot(db, type, itemUid, cancellationToken);
						}*/
					}
					else if (type.HierarchyType == HierarchyType.Items)
					{
						var closureTable = new ClosureTableHandler(db, type);

						if (await closureTable.Insert(itemUid, item.ParentUid, cancellationToken) == false)
						{
							return new ApiResult { Success = false, Errors = closureTable.Errors };
						}
					}
				}

				// insert fields
				await _fieldDataRepository.Insert(manageFieldDataRequest, cancellationToken);

				// todo: events

				scope.Commit();

				return new ApiResult { Uid = itemUid };
			}
		}

		/*private static async Task LinkToDefaultRoot(DbContext db, ClassifierType type, Guid itemUid, CancellationToken cancellationToken)
		{
			var defaultRoot = await GetDefaultRoot(db, type, cancellationToken);

			// todo: insert default root group?
			if (defaultRoot != null)
			{
				await db.GetTable<DbClassifierLink>()
					.Value(x => x.GroupUid, defaultRoot.Uid)
					.Value(x => x.ItemUid, itemUid)
					.InsertAsync(cancellationToken);
			}
		}

		// todo: move to ClassifierGroupService.GetDefaultRoot()
		private static async Task<DbClassifierGroup> GetDefaultRoot(DbContext db, ClassifierType type, CancellationToken cancellationToken)
		{
			return await (
				from @group in db.GetTable<DbClassifierGroup>()
				where @group.TypeUid == type.Uid && @group.Code == ClassifierTree.DefaultCode
				select @group
			).SingleOrDefaultAsync(cancellationToken);
		}*/

		// todo: move to ClassifierGroupService.GetRoot()
		public static async Task<DbClassifierGroup> GetRoot(DbContext db, Guid groupUid, CancellationToken cancellationToken)
		{
			return await (
				from closure in db.GetTable<DbClassifierClosure>()
					.Where(x => x.ChildUid == groupUid)
				join maxLevel in (
					from path in db.GetTable<DbClassifierClosure>()
						.Where(x => x.ChildUid == groupUid)
					group path by path.ChildUid
					into parents
					select parents.Max(x => x.Level)) on closure.Level equals maxLevel
				join parent in db.GetTable<DbClassifierGroup>() on closure.ParentUid equals parent.Uid
				select parent
			).SingleAsync(cancellationToken);
		}
	}
}
