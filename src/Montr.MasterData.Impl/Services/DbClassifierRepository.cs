﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Core.Models;
using Montr.Core.Services;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;
using Montr.Metadata.Models;
using Montr.Metadata.Services;

namespace Montr.MasterData.Impl.Services
{
	public class DbClassifierRepository : IRepository<Classifier>
	{
		private readonly IDbContextFactory _dbContextFactory;
		private readonly IClassifierTypeService _classifierTypeService;
		private readonly IClassifierTypeMetadataService _metadataService;
		private readonly IFieldDataRepository _fieldDataRepository;

		public DbClassifierRepository(IDbContextFactory dbContextFactory,
			IClassifierTypeService classifierTypeService,
			IClassifierTypeMetadataService metadataService,
			IFieldDataRepository fieldDataRepository)
		{
			_dbContextFactory = dbContextFactory;
			_classifierTypeService = classifierTypeService;
			_metadataService = metadataService;
			_fieldDataRepository = fieldDataRepository;
		}

		public async Task<SearchResult<Classifier>> Search(SearchRequest searchRequest, CancellationToken cancellationToken)
		{
			var request = (ClassifierSearchRequest)searchRequest ?? throw new ArgumentNullException(nameof(searchRequest));

			var type = await _classifierTypeService.Get(request.TypeCode, cancellationToken);

			using (var db = _dbContextFactory.Create())
			{
				IQueryable<DbClassifier> query = null;

				if (type.HierarchyType == HierarchyType.None)
				{
					// no-op
				}
				else if (type.HierarchyType == HierarchyType.Groups)
				{
					if (request.GroupUid != null)
					{
						if (request.Depth == null || request.Depth == "0") // todo: use constant
						{
							query = from trees in db.GetTable<DbClassifierTree>()
								join childrenGroups in db.GetTable<DbClassifierGroup>() on trees.Uid equals childrenGroups.TreeUid
								join links in db.GetTable<DbClassifierLink>() on childrenGroups.Uid equals links.GroupUid
								join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
								where trees.TypeUid == type.Uid &&
									trees.Uid == request.TreeUid &&
									childrenGroups.Uid == request.GroupUid
								select c;
						}
						else
						{
							query = from trees in db.GetTable<DbClassifierTree>()
								join parentGroups in db.GetTable<DbClassifierGroup>() on trees.Uid equals parentGroups.TreeUid
								join closures in db.GetTable<DbClassifierClosure>() on parentGroups.Uid equals closures.ParentUid
								join childrenGroups in db.GetTable<DbClassifierGroup>() on closures.ChildUid equals childrenGroups.Uid
								join links in db.GetTable<DbClassifierLink>() on childrenGroups.Uid equals links.GroupUid
								join c in db.GetTable<DbClassifier>() on links.ItemUid equals c.Uid
								where trees.TypeUid == type.Uid &&
								      trees.Uid == request.TreeUid &&
								      parentGroups.Uid == request.GroupUid
								select c;
						}
					}
				}
				else if (type.HierarchyType == HierarchyType.Items)
				{
					if (request.GroupUid != null)
					{
						if (request.Depth == null || request.Depth == "0") // todo: use enum or constant
						{
							query = from parent in db.GetTable<DbClassifier>()
									join @class in db.GetTable<DbClassifier>() on parent.Uid equals @class.ParentUid
									where parent.TypeUid == type.Uid && parent.Uid == request.GroupUid
									select @class;
						}
						else
						{
							query = from parent in db.GetTable<DbClassifier>()
									join closures in db.GetTable<DbClassifierClosure>() on parent.Uid equals closures.ParentUid
									join @class in db.GetTable<DbClassifier>() on closures.ChildUid equals @class.Uid
									where parent.TypeUid == type.Uid && parent.Uid == request.GroupUid && closures.Level > 0
									select @class;
						}
					}
				}

				if (query == null)
				{
					query = from c in db.GetTable<DbClassifier>()
							where c.TypeUid == type.Uid
							select c;
				}

				if (request.Uid != null)
				{
					query = query.Where(x => x.Uid == request.Uid);
				}

				if (request.Uids != null)
				{
					query = query.Where(x => request.Uids.Contains(x.Uid));
				}

				if (request.SearchTerm != null)
				{
					query = query.Where(x => SqlExpr.ILike(x.Name, "%" + request.SearchTerm + "%"));

					// query = query.Where(x => Sql.Like(x.Name, "%" + request.SearchTerm + "%"));
					// query = query.Where(x => x.Name.Contains(request.SearchTerm));
				}

				/*if (type.Code == "numerator")
				{
					var joined = from c in query
						join t in db.GetTable<DbNumerator>() on c.Uid equals t.Uid
						select new { c, t };
				}*/

				var data = await Materialize(
					query.Apply(request, x => x.Code), type, cancellationToken);

				// todo: add test
				if (request.FocusUid.HasValue && data.Any(x => x.Uid == request.FocusUid) == false)
				{
					var focused = await Materialize(
							query.Where(x => x.Uid == request.FocusUid),
							type, cancellationToken);

					data.InsertRange(0, focused);
				}

				// todo: preload fields for multiple items
				if (request.IncludeFields)
				{
					var metadata = await _metadataService.GetMetadata(type, cancellationToken);

					foreach (var item in data)
					{
						var fields = await _fieldDataRepository.Search(new FieldDataSearchRequest
						{
							Metadata = metadata,
							EntityTypeCode = Classifier.TypeCode,
							// ReSharper disable once PossibleInvalidOperationException
							EntityUids = new[] { item.Uid.Value }
						}, cancellationToken);

						item.Fields = fields.Rows.SingleOrDefault();
					}
				}

				return new SearchResult<Classifier>
				{
					TotalCount = query.GetTotalCount(request),
					Rows = data
				};
			}
		}

		private static async Task<List<Classifier>> Materialize(IQueryable<DbClassifier> query,
			ClassifierType type, CancellationToken cancellationToken)
		{
			return await query
				.Select(x => new Classifier
				{
					Type = type.Code,
					Uid = x.Uid,
					StatusCode = x.StatusCode,
					Code = x.Code,
					Name = x.Name,
					ParentUid = x.ParentUid,
					Url = $"/classifiers/{type.Code}/edit/{x.Uid}"
				})
				.ToListAsync(cancellationToken);
		}
	}
}

