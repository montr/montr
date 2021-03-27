using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
using Montr.Idx.Services;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Services;

namespace Montr.Idx.Impl.Services
{
	public class DbUserRepository : DbClassifierRepository<User>
	{
		private readonly IUserManager _userManager;

		public DbUserRepository(
			IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService,
			IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator,
			IUserManager userManager)
			: base(
				dbContextFactory,
				classifierTypeService,
				classifierTreeService,
				metadataService,
				fieldDataRepository,
				numberGenerator)
		{
			_userManager = userManager;
		}

		protected override async Task<SearchResult<Classifier>> SearchInternal(DbContext db,
			ClassifierType type, ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var classifiers = BuildQuery(db, type, request);

			var users = db.GetTable<DbUser>().AsQueryable();

			if (request is UserSearchRequest userRequest)
			{
				if (userRequest.UserName != null)
				{
					users = users.Where(x => x.UserName == userRequest.UserName);
				}
			}
			var joined = from classifier in classifiers
				join user in users on classifier.Uid equals user.Id
				select new DbItem { Classifier = classifier, User = user };

			// todo: fix paging - map column to expression
			request.SortColumn ??= nameof(Classifier.Code);
			request.SortColumn = nameof(DbItem.Classifier) + "." + request.SortColumn;

			var data = await joined
				.Apply(request, x => x.Classifier.Code)
				.Select(x => Materialize(type, x))
				.Cast<Classifier>()
				.ToListAsync(cancellationToken);

			return new SearchResult<Classifier>
			{
				TotalCount = joined.GetTotalCount(request),
				Rows = data
			};
		}

		private User Materialize(ClassifierType type, DbItem dbItem)
		{
			var item = base.Materialize(type, dbItem.Classifier);

			var dbUser = dbItem.User;

			item.UserName = dbUser.UserName;
			item.FirstName = dbUser.FirstName;
			item.LastName = dbUser.LastName;
			item.PhoneNumber = dbUser.PhoneNumber;
			item.Email = dbUser.Email;

			return item;
		}

		public override async Task<ApiResult> Insert(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Insert(item, cancellationToken);

			if (result.Success)
			{
				return await _userManager.Create((User) item, cancellationToken);
			}

			return result;
		}

		public override async Task<ApiResult> Update(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Update(item, cancellationToken);

			if (result.Success)
			{
				// todo: restore optimistic concurrency check (?)
				// ReSharper disable once PossibleInvalidOperationException
				var role = await _userManager.Get(item.Uid.Value, cancellationToken);

				return await _userManager.Update(role, cancellationToken);
			}

			return result;
		}

		public override async Task<ApiResult> Delete(DeleteClassifier request, CancellationToken cancellationToken)
		{
			foreach (var uid in request.Uids)
			{
				var item = await _userManager.Get(uid, cancellationToken);

				var result = await _userManager.Delete(item, cancellationToken);

				if (result.Success == false) return result;
			}

			return await base.Delete(request, cancellationToken);
		}

		private class DbItem
		{
			public DbClassifier Classifier { get; init; }

			public DbUser User { get; init; }
		}
	}
}
