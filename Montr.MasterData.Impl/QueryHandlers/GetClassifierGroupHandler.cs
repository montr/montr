using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Queries;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.QueryHandlers
{
	public class GetClassifierGroupHandler : IRequestHandler<GetClassifierGroup, ClassifierGroup>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;

		public GetClassifierGroupHandler(IDbContextFactory dbContextFactory, IClassifierTypeService classifierTypeService)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
		}

		public async Task<ClassifierGroup> Handle(GetClassifierGroup command, CancellationToken cancellationToken)
		{
			if (command.UserUid == Guid.Empty) throw new InvalidOperationException("User is required.");
			if (command.CompanyUid == Guid.Empty) throw new InvalidOperationException("Company is required.");

			var typeCode = command.TypeCode ?? throw new ArgumentNullException(nameof(command.TypeCode));
			var treeCode = command.TreeCode ?? throw new ArgumentNullException(nameof(command.TreeCode));

			var type = await _classifierTypeService.GetClassifierType(command.CompanyUid, typeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				if (type.HierarchyType == HierarchyType.Groups)
				{
					var tree = await db.GetTable<DbClassifierTree>()
						.SingleAsync(x => x.TypeUid == type.Uid && x.Code == treeCode, cancellationToken);

					var item = await db.GetTable<DbClassifierGroup>()
						.SingleAsync(
							x => x.TreeUid == tree.Uid && x.Uid == command.Uid, cancellationToken);

					return new ClassifierGroup
					{
						Uid = item.Uid,
						ParentUid = item.ParentUid,
						Code = item.Code,
						Name = item.Name
					};
				}

				throw new InvalidOperationException("Groups not supported for classifiers with hierarchy type " + type.HierarchyType);
			}
		}
	}
}
