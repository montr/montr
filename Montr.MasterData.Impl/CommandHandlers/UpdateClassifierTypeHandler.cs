using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class UpdateClassifierTypeHandler : IRequestHandler<UpdateClassifierType, int>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;

		public UpdateClassifierTypeHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
		}

		public async Task<int> Handle(UpdateClassifierType request, CancellationToken cancellationToken)
		{
			if (request.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (request.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var item = request.Item ?? throw new ArgumentNullException(nameof(request.Item));

			using (var scope = _unitOfWorkFactory.Create())
			{
				int result;

				using (var db = _dbContextFactory.Create())
				{
					result = await db.GetTable<DbClassifierType>()
						.Where(x => x.Uid == item.Uid && x.CompanyUid == request.CompanyUid)
						.Set(x => x.Code, item.Code)
						.Set(x => x.Name, item.Name)
						.Set(x => x.Description, item.Description)
						.Set(x => x.HierarchyType, item.HierarchyType.ToString())
						.UpdateAsync(cancellationToken);

					if (item.HierarchyType == HierarchyType.Groups)
					{
						var tree = db.GetTable<DbClassifierTree>()
							.SingleOrDefault(x =>
								x.TypeUid == item.Uid &&
								x.Code == ClassifierTree.DefaultTreeCode);

						if (tree == null)
						{
							await db.GetTable<DbClassifierTree>()
								.Value(x => x.Uid, Guid.NewGuid())
								.Value(x => x.TypeUid, item.Uid)
								.Value(x => x.Code, ClassifierTree.DefaultTreeCode)
								.Value(x => x.Name, item.Name)
								.InsertAsync(cancellationToken);
						}
					}
				}

				// todo: (события)

				scope.Commit();

				return result;
			}
		}
	}
}
