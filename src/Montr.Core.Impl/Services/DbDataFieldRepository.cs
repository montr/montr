using System;
using System.Collections.Generic;
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
		private readonly IJsonSerializer _jsonSerializer;

		public DbDataFieldRepository(IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<SearchResult<DataField>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (MetadataSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db
					.GetTable<DbFieldMeta>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode);

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				var withPaging = request.PageSize > 0;

				var paged = withPaging ? query.Apply(request, x => x.DisplayOrder) : query.OrderBy(x => x.DisplayOrder);

				var data = await paged
					.Select(x => x)
					.ToListAsync(cancellationToken);

				var result = new List<DataField>();

				foreach (var dbField in data)
				{
					// todo: use factory
					var field = (DataField) Activator.CreateInstance(DataFieldTypes.Map[dbField.TypeCode]);

					field.Uid = dbField.Uid;
					field.Key = dbField.Key;
					field.Name = dbField.Name;
					field.Description = dbField.Description;
					field.Placeholder = dbField.Placeholder;
					field.Icon = dbField.Icon;
					field.Active = dbField.IsActive;
					field.System = dbField.IsSystem;
					field.Readonly = dbField.IsReadonly;
					field.Required = dbField.IsRequired;
					field.DisplayOrder = dbField.DisplayOrder;

					var propertiesType = field.GetPropertiesType();

					if (propertiesType != null && dbField.Extra != null)
					{
						var properties = _jsonSerializer.Deserialize(dbField.Extra, propertiesType);

						field.SetProperties(properties);
					}

					result.Add(field);
				}

				return new SearchResult<DataField>
				{
					TotalCount = withPaging ? query.Count() : (int?)null,
					Rows = result
				};
			}
		}
	}
}
