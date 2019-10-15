using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using MediatR;
using Montr.Core.Commands;
using Montr.Core.Impl.Entities;
using Montr.Core.Impl.Services;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.CommandHandlers
{
	public class ImportLocaleStringListHandler :  IRequestHandler<ImportLocaleStringList, ApiResult>
	{
		private readonly IUnitOfWorkFactory _unitOfWorkFactory;
		private readonly IDbContextFactory _dbContextFactory;
		private readonly LocaleStringSerializer _serializer;

		public ImportLocaleStringListHandler(IUnitOfWorkFactory unitOfWorkFactory, IDbContextFactory dbContextFactory, LocaleStringSerializer serializer)
		{
			_unitOfWorkFactory = unitOfWorkFactory;
			_dbContextFactory = dbContextFactory;
			_serializer = serializer;
		}

		public async Task<ApiResult> Handle(ImportLocaleStringList request, CancellationToken cancellationToken)
		{
			var list = await _serializer.Deserialize(request.Stream, cancellationToken);

			var affected = 0;

			using (var scope = _unitOfWorkFactory.Create())
			{
				using (var db = _dbContextFactory.Create())
				{
					foreach (var locale in list)
					{
						foreach (var module in locale.Modules)
						{
							var deleted = await db.GetTable<DbLocaleString>()
								.Where(x => x.Locale == locale.Locale && x.Module == module.Module)
								.DeleteAsync(cancellationToken);

							var inserted = module.Items.Select(x => new DbLocaleString
							{
								Locale = locale.Locale,
								Module = module.Module,
								Key = x.Key,
								Value = x.Value
							});

							var copied = db.BulkCopy(inserted);

							// todo: write audit logs
						}
					}
				}

				scope.Commit();
			}

			return new ApiResult { AffectedRows = affected };
		}
	}
}
