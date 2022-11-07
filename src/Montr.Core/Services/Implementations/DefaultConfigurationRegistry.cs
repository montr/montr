using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Montr.Core.Models;

namespace Montr.Core.Services.Implementations
{
	public class DefaultConfigurationRegistry : IConfigurationRegistry
	{
		private readonly ConcurrentDictionary<Type, object> _entityConfigs = new();

		public void Configure<TEntity>(Action<IConditionalEntityConfiguration<TEntity>> config)
		{
			var configuration = GetEntityConfiguration<TEntity>();

			config?.Invoke(configuration);
		}

		public IEnumerable<T> GetItems<TEntity, T>(TEntity entity) where T : IConfigurationItem, new()
		{
			var configuration = GetEntityConfiguration<TEntity>();

			return configuration.GetItems<T>(entity);
		}

		private ConditionalEntityConfiguration<TEntity> GetEntityConfiguration<TEntity>()
		{
			return (ConditionalEntityConfiguration<TEntity>)_entityConfigs
				.GetOrAdd(typeof(TEntity), _ => new ConditionalEntityConfiguration<TEntity>());
		}

		private class ConditionalEntityConfiguration<TEntity> : IConditionalEntityConfiguration<TEntity>
		{
			private readonly List<ItemInfoList<TEntity>> _items = new();

			public IEntityConfiguration<TEntity> When(Predicate<TEntity> condition)
			{
				var items = new List<ItemInfo<TEntity>>();

				_items.Add(new ItemInfoList<TEntity>(condition, items));

				return new EntityConfiguration<TEntity>(items);
			}

			public IEntityConfiguration<TEntity> Add<T>(Action<TEntity, T> init) where T : IConfigurationItem, new()
			{
				_items.Add(new ItemInfoList<TEntity>(_ => true, new[] { new ItemInfo<TEntity, T>(init) }));

				return this;
			}

			public IEnumerable<T> GetItems<T>(TEntity entity) where T : IConfigurationItem, new()
			{
				return _items.SelectMany(container => container.GetItems<T>(entity)).Cast<T>();
			}
		}

		private class EntityConfiguration<TEntity> : IEntityConfiguration<TEntity>
		{
			private readonly ICollection<ItemInfo<TEntity>> _items;

			public EntityConfiguration(ICollection<ItemInfo<TEntity>> items)
			{
				_items = items;
			}

			public IEntityConfiguration<TEntity> Add<T>(Action<TEntity, T> init) where T : IConfigurationItem, new()
			{
				_items.Add(new ItemInfo<TEntity, T>(init));

				return this;
			}
		}

		private abstract class ItemInfo<TEntity>
		{
			public abstract bool TryGetItem<T>(TEntity entity, out IConfigurationItem item);
		}

		private class ItemInfo<TEntity, TITem> : ItemInfo<TEntity> where TITem : IConfigurationItem, new()
		{
			private readonly Action<TEntity, TITem> _itemInit;

			public ItemInfo(Action<TEntity, TITem> itemInit)
			{
				_itemInit = itemInit;
			}

			public override bool TryGetItem<T>(TEntity entity, out IConfigurationItem item)
			{
				if (typeof(TITem).IsAssignableTo(typeof(T)))
				{
					var result = new TITem();

					_itemInit?.Invoke(entity, result);

					item = result;
					return true;
				}

				item = null;
				return false;
			}
		}

		private class ItemInfoList<TEntity>
		{
			private readonly Predicate<TEntity> _condition;
			private readonly ICollection<ItemInfo<TEntity>> _items;

			public ItemInfoList(Predicate<TEntity> condition, ICollection<ItemInfo<TEntity>> items)
			{
				_condition = condition;
				_items = items;
			}

			public IEnumerable<IConfigurationItem> GetItems<T>(TEntity entity)
			{
				if (_condition?.Invoke(entity) == false) yield break;

				foreach (var itemInfo in _items)
				{
					if (itemInfo.TryGetItem<T>(entity, out var item))
					{
						yield return item;
					}
				}
			}
		}
	}
}
