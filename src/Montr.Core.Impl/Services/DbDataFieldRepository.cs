using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Impl.Entities;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;

namespace Montr.Core.Impl.Services
{
	public class DbDataFieldRepository : IRepository<DataField>
	{
		private readonly IDbContextFactory _dbContextFactory;

		public DbDataFieldRepository(IDbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public async Task<SearchResult<DataField>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (MetadataSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var all = db
					.GetTable<DbFieldMeta>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode);

				var withPaging = request.PageSize > 0;

				var paged = withPaging ? all.Apply(request, x => x.Key) : all;

				var data = await paged
					.Select(x => new StringField // todo: use factory
					{
						Key = x.Key,
						Name = x.Name,
						Description = x.Description,
						Placeholder = x.Placeholder,
						Icon = x.Icon,
						Active = x.IsActive,
						System = x.IsSystem,
						Readonly = x.IsReadonly,
						Required = x.IsRequired,
					})
					.ToListAsync(cancellationToken);

				return new SearchResult<DataField>
				{
					TotalCount = withPaging ? all.Count() : (int?)null,
					Rows = data.Cast<DataField>().ToList()
				};
			}
		}
	}
}
