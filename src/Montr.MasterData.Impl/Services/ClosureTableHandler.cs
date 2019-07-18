using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Impl.Services
{
	public class ClosureTableHandler
	{
		private readonly DbContext _db;
		private readonly ClassifierType _type;

		public ClosureTableHandler(DbContext db, ClassifierType type)
		{
			if (type.HierarchyType == HierarchyType.None)
			{
				throw new InvalidOperationException($"Hierarchy type {type.HierarchyType} is not suitable to build closure table.");
			}

			_db = db;
			_type = type;
		}

		public IList<ApiResultError> Errors { get; } = new List<ApiResultError>();

		public async Task<bool> Insert(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
			await ValidateSameTree(itemUid, parentUid, cancellationToken);

			if (Errors.Count > 0) return false;

			var closure = _db.GetTable<DbClassifierClosure>();

			// insert self closure with level 0
			await closure
				.Value(x => x.ParentUid, itemUid)
				.Value(x => x.ChildUid, itemUid)
				.Value(x => x.Level, 0)
				.InsertAsync(cancellationToken);

			if (parentUid != null)
			{
				// insert parent closures with level + 1
				// todo: reuse insert from Update method
				var insertable = await closure
					.Where(x => x.ChildUid == parentUid)
					.Select(x => new DbClassifierClosure
					{
						ParentUid = x.ParentUid,
						ChildUid = itemUid,
						Level = (short)(x.Level + 1)
					})
					.ToListAsync(cancellationToken);

				_db.BulkCopy(insertable);
			}

			return true;
		}

		public async Task<bool> Update(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
			await ValidateSameTree(itemUid, parentUid, cancellationToken);
			await ValidateCyclicDependency(itemUid, parentUid, cancellationToken);

			if (Errors.Count > 0) return false;

			var closure = _db.GetTable<DbClassifierClosure>();
			
			// delete moved node with children from old parent, i.e.:		
			// take all children of parents nodes for node to delete (parent)
			// take all children nodes for node to delete (child)
			// cross join all (parent) with (child) and remove it

			var deletable = from parent in closure
				where parent.ChildUid == itemUid && parent.Level > 0
				from child in closure
				where child.ParentUid == itemUid // && child.Level > 0
				join cross in closure
					on new { parent.ParentUid, child.ChildUid }
					equals new { cross.ParentUid, cross.ChildUid }
				select cross;

			await (from node in closure
				join x in deletable
					on new { node.ParentUid, node.ChildUid, node.Level }
					equals new { x.ParentUid, x.ChildUid, x.Level }
				select node).DeleteAsync(cancellationToken);

			// insert moved node with all children to new parent
			if (parentUid.HasValue)
			{
				var insertable = await (
					from parent in closure where parent.ChildUid == parentUid
					from child in closure where child.ParentUid == itemUid
					select new DbClassifierClosure
					{
						ParentUid = parent.ParentUid,
						ChildUid = child.ChildUid,
						Level = (short)(parent.Level + child.Level + 1)
					}).ToListAsync(cancellationToken);

				_db.BulkCopy(insertable);
			}

			return true;
		}

		public async Task<bool> Delete(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
			await ValidateSameTree(itemUid, parentUid, cancellationToken);

			if (Errors.Count > 0) return false;

			var closure = _db.GetTable<DbClassifierClosure>();

			// move children to parent with level - 1
			// (if no parent - all children stay the same)
			if (parentUid != null)
			{
				// take all children of parents nodes for node to delete (parent)
				// take all children nodes for node to delete (child)
				// cross join all (parent) with (child) and set its level = level - 1 
				// (make all children of deleted node one level closer to deleted node parents)
				await (from parent in closure
						where parent.ChildUid == itemUid && parent.Level > 0
						from child in closure
						where child.ParentUid == itemUid && child.Level > 0
						join updatable in closure
							on new { parent.ParentUid, child.ChildUid }
							equals new { updatable.ParentUid, updatable.ChildUid }
						select updatable)
					.AsUpdatable()
					.Set(x => x.Level, x => x.Level - 1)
					.UpdateAsync(cancellationToken);
			}

			// delete unused closure nodes
			await closure
				.Where(x => x.ParentUid == itemUid)
				.DeleteAsync(cancellationToken);

			return true;
		}

		// todo: validate same type
		/*private async Task ValidateSameType(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
		}*/

		private async Task ValidateSameTree(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
			if (_type.HierarchyType == HierarchyType.Groups && parentUid != null)
			{
				var groups = _db.GetTable<DbClassifierGroup>();

				var query =
					from item in groups
					join parent in groups on item.TreeUid equals parent.TreeUid
					where item.Uid == itemUid && parent.Uid == parentUid
					select item;

				if (await query.AnyAsync(cancellationToken) == false)
				{
					// todo: translate errors
					Errors.Add(new ApiResultError
					{
						Key = string.Empty,
						Messages = new[]
						{
							"Item and parent should belongs to the same tree."
						}
					});
				}
			}
		}

		private async Task ValidateCyclicDependency(Guid itemUid, Guid? parentUid, CancellationToken cancellationToken)
		{
			if (parentUid != null)
			{
				var closure = _db.GetTable<DbClassifierClosure>();

				var query =
					from c in closure
					where c.ChildUid == parentUid && c.ParentUid == itemUid
					select c;

				if (await query.AnyAsync(cancellationToken))
				{
					// todo: translate errors
					Errors.Add(new ApiResultError
					{
						Key = "parentUid",
						Messages = new[]
						{
							"Cyclic dependency detected."
						}
					});
				}
			}
		}
	}
}
