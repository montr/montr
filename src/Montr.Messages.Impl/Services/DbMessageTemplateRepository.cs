using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Commands;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Impl.Services;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Messages.Impl.Entities;
using Montr.Messages.Models;
using Montr.Metadata.Services;

namespace Montr.Messages.Impl.Services
{
	public class DbMessageTemplateRepository : DbClassifierRepository<MessageTemplate>
	{
		public DbMessageTemplateRepository(IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService, IClassifierTreeService classifierTreeService,
			IClassifierTypeMetadataService metadataService, IFieldDataRepository fieldDataRepository,
			INumberGenerator numberGenerator) : base(dbContextFactory,
			classifierTypeService, classifierTreeService, metadataService, fieldDataRepository, numberGenerator)
		{
		}

		protected override async Task<SearchResult<Classifier>> SearchInternal(DbContext db,
			ClassifierType type, ClassifierSearchRequest request, CancellationToken cancellationToken)
		{
			var classifiers = BuildQuery(db, type, request);

			var dbMessageTemplates = db.GetTable<DbMessageTemplate>().AsQueryable();

			var joined = from classifier in classifiers
				join dbMessageTemplate in dbMessageTemplates on classifier.Uid equals dbMessageTemplate.Uid
				select new DbItem { Classifier = classifier, MessageTemplate = dbMessageTemplate };

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

		private MessageTemplate Materialize(ClassifierType type, DbItem dbItem)
		{
			var item = base.Materialize(type, dbItem.Classifier);

			var dbMessageTemplate = dbItem.MessageTemplate;

			item.Subject = dbMessageTemplate.Subject;
			item.Body = dbMessageTemplate.Body;

			return item;
		}

		protected override async Task<ApiResult> InsertInternal(DbContext db,
			ClassifierType type, Classifier item, CancellationToken cancellationToken = default)
		{
			var result = await base.InsertInternal(db, type, item, cancellationToken);

			if (result.Success)
			{
				var template = (MessageTemplate)item;

				await db.GetTable<DbMessageTemplate>()
					.Value(x => x.Uid, item.Uid)
					.Value(x => x.Subject, template.StatusCode)
					.Value(x => x.Body, template.Body)
					.InsertAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> UpdateInternal(DbContext db,
			ClassifierType type, ClassifierTree tree, Classifier item, CancellationToken cancellationToken = default)
		{
			var result = await base.UpdateInternal(db, type, tree, item, cancellationToken);

			if (result.Success)
			{
				var template = (MessageTemplate)item;

				await db.GetTable<DbMessageTemplate>()
					.Where(x => x.Uid == item.Uid)
					.Set(x => x.Subject, template.Subject)
					.Set(x => x.Body, template.Body)
					.UpdateAsync(cancellationToken);
			}

			return result;
		}

		protected override async Task<ApiResult> DeleteInternal(DbContext db,
			ClassifierType type, DeleteClassifier request, CancellationToken cancellationToken = default)
		{
			// todo: validate template is not used
			await db.GetTable<DbMessageTemplate>()
				.Where(x => request.Uids.Contains(x.Uid))
				.DeleteAsync(cancellationToken);

			return await base.DeleteInternal(db, type, request, cancellationToken);
		}

		private class DbItem
		{
			public DbClassifier Classifier { get; init; }

			public DbMessageTemplate MessageTemplate { get; init; }
		}
	}
}
