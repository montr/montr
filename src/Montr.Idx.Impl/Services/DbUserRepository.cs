using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Idx.Impl.Entities;
using Montr.Idx.Models;
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
		private readonly ILogger<DbUserRepository> _logger;
		private readonly UserManager<DbUser> _userManager;

		public DbUserRepository(
			ILogger<DbUserRepository> logger,
			IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService,
			IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator,
			UserManager<DbUser> userManager)
			: base(
				dbContextFactory,
				classifierTypeService,
				classifierTreeService,
				metadataService,
				fieldDataRepository,
				numberGenerator)
		{
			_logger = logger;
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
					// todo: use normalized username or use user manager
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
				var user = (User) item;

				var dbUser = new DbUser
				{
					Id = user.Uid ?? throw new ArgumentException("Id of created user can not be empty as it referencing classifier.", nameof(user)),
					UserName = user.UserName,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					PhoneNumber = user.PhoneNumber
				};

				var identityResult = user.Password == null
					? await _userManager.CreateAsync(dbUser)
					: await _userManager.CreateAsync(dbUser, user.Password);

				result = identityResult.ToApiResult();

				if (result.Success)
				{
					var message = user.Password == null
						? "Created user {userName} without password."
						: "Created user {userName} with password.";

					_logger.LogInformation(message, dbUser.UserName);

					result.Uid = dbUser.Id;
				}
			}

			return result;
		}

		public override async Task<ApiResult> Update(Classifier item, CancellationToken cancellationToken)
		{
			var result = await base.Update(item, cancellationToken);

			if (result.Success)
			{
				var user = (User) item;

				var dbUser = await _userManager.FindByIdAsync(item.Uid.ToString());

				// todo: restore optimistic concurrency check (?)
				// dbUser.ConcurrencyStamp = user.ConcurrencyStamp;

				dbUser.UserName = user.UserName;
				dbUser.LastName = user.LastName;
				dbUser.FirstName = user.FirstName;
				dbUser.Email = user.Email;
				dbUser.PhoneNumber = user.PhoneNumber;

				var identityResult =  await _userManager.UpdateAsync(dbUser);

				result = identityResult.ToApiResult();

				if (result.Success)
				{
					_logger.LogInformation("Updated user {userName}.", dbUser.UserName);

					result.Uid = dbUser.Id;
					result.ConcurrencyStamp = dbUser.ConcurrencyStamp;
				}
			}

			return result;
		}

		public override async Task<ApiResult> Delete(DeleteClassifier request, CancellationToken cancellationToken)
		{
			foreach (var uid in request.Uids)
			{
				var dbUser = await _userManager.FindByIdAsync(uid.ToString());

				// todo: restore optimistic concurrency check (?)
				// dbUser.ConcurrencyStamp = user.ConcurrencyStamp;

				var result = await _userManager.DeleteAsync(dbUser);

				if (result.Succeeded == false) return result.ToApiResult();

				_logger.LogInformation("Deleted user {userName}.", dbUser.UserName);
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
