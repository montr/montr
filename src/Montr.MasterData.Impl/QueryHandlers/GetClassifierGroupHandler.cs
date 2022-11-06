using System;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using MediatR;
using Montr.Core.Services;
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

		public async Task<ClassifierGroup> Handle(GetClassifierGroup request, CancellationToken cancellationToken)
		{
			var typeCode = request.TypeCode ?? throw new ArgumentNullException(nameof(request.TypeCode));

			var type = await _classifierTypeService.Get(typeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				if (type.HierarchyType == HierarchyType.Groups)
				{
					var tree = await db.GetTable<DbClassifierTree>()
						.SingleAsync(x => x.TypeUid == type.Uid && x.Uid == request.TreeUid, cancellationToken);

					var item = await db.GetTable<DbClassifierGroup>()
						.SingleAsync(x => x.TreeUid == tree.Uid && x.Uid == request.Uid, cancellationToken);

					return new ClassifierGroup
					{
						Uid = item.Uid,
						TreeUid = item.TreeUid,
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
