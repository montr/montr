using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.Metadata.Impl.Entities;
using Montr.Metadata.Models;

namespace Montr.Core.Impl.Services
{
	public class DbFieldMetadataRepository : IRepository<FieldMetadata>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IJsonSerializer _jsonSerializer;

		public DbFieldMetadataRepository(IDbContextFactory dbContextFactory, IJsonSerializer jsonSerializer)
		{
			_dbContextFactory = dbContextFactory;
			_jsonSerializer = jsonSerializer;
		}

		public async Task<SearchResult<FieldMetadata>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (MetadataSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			using (var db = _dbContextFactory.Create())
			{
				var query = db
					.GetTable<DbFieldMetadata>()
					.Where(x => x.EntityTypeCode == request.EntityTypeCode);

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				if (request.IsSystem != null)
				{
					query = query.Where(x => x.IsSystem == request.IsSystem);
				}

				if (request.IsActive != null)
				{
					query = query.Where(x => x.IsActive == request.IsActive);
				}

				var withPaging = request.PageSize > 0;

				var paged = withPaging ? query.Apply(request, x => x.DisplayOrder) : query.OrderBy(x => x.DisplayOrder);

				var data = await paged
					.Select(x => x)
					.ToListAsync(cancellationToken);

				var result = new List<FieldMetadata>();

				foreach (var dbField in data)
				{
					// todo: use factory
					var field = (FieldMetadata) Activator.CreateInstance(DataFieldTypes.Map[dbField.TypeCode]);

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

					if (propertiesType != null && dbField.Props != null)
					{
						var properties = _jsonSerializer.Deserialize(dbField.Props, propertiesType);

						field.SetProperties(properties);
					}

					result.Add(field);
				}

				return new SearchResult<FieldMetadata>
				{
					TotalCount = withPaging ? query.Count() : (int?)null,
					Rows = result
				};
			}
		}
	}
}
