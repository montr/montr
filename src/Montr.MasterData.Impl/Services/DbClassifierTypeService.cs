using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.MasterData.Impl.CommandHandlers;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbClassifierTypeService: IClassifierTypeService
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IRepository<ClassifierType> _repository;

		public DbClassifierTypeService(IDbContextFactory dbContextFactory, IRepository<ClassifierType> repository)
		{
			_dbContextFactory = dbContextFactory;
			_repository = repository;
		}

		public async Task<ClassifierType> TryGet(string typeCode, CancellationToken cancellationToken)
		{
			var result = await _repository.Search(new ClassifierTypeSearchRequest
			{
				Code = typeCode ?? throw new ArgumentNullException(nameof(typeCode)),
				PageNo = 0,
				PageSize = 1,
				SkipPaging = true
			}, cancellationToken);

			return result.Rows.SingleOrDefault();
		}

		public async Task<ClassifierType> Get(string typeCode, CancellationToken cancellationToken)
		{
			var result = await TryGet(typeCode, cancellationToken);

			if (result == null)
			{
				throw new InvalidOperationException($"Classifier type \"{typeCode}\" not found.");
			}

			return result;
		}

		public async Task<ApiResult> Insert(ClassifierType item, CancellationToken cancellationToken)
		{
			var itemUid = Guid.NewGuid();

			// todo: validation and limits
			// todo: reserved codes (add, new etc. can conflict with routing)

			using (var db = _dbContextFactory.Create())
			{
				var validator = new ClassifierTypeValidator(db);

				if (await validator.ValidateInsert(item, cancellationToken) == false)
				{
					return new ApiResult { Success = false, Errors = validator.Errors };
				}

				// todo: change date

				await db.GetTable<DbClassifierType>()
					.Value(x => x.Uid, itemUid)
					.Value(x => x.Code, item.Code)
					.Value(x => x.Name, item.Name)
					.Value(x => x.Description, item.Description)
					.Value(x => x.HierarchyType, item.HierarchyType.ToString())
					.Value(x => x.IsSystem, item.IsSystem)
					.InsertAsync(cancellationToken);

				if (item.HierarchyType == HierarchyType.Groups)
				{
					var treeUid = Guid.NewGuid();

					await db.GetTable<DbClassifierTree>()
						.Value(x => x.Uid, treeUid)
						.Value(x => x.TypeUid, itemUid)
						.Value(x => x.Code, ClassifierTree.DefaultCode)
						.Value(x => x.Name, item.Name)
						.InsertAsync(cancellationToken);
				}
			}

			return new ApiResult { Uid = itemUid };
		}
	}
}
