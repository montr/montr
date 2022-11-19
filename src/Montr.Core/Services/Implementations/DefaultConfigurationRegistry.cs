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

		public IEnumerable<TItem> GetItems<TItem>(Type ofEntity, object entity) where TItem : IConfigurationItem, new()
		{
			var configuration = GetEntityConfiguration(ofEntity);

			return configuration.GetItems<TItem>(entity);
		}

		public IEnumerable<TItem> GetItems<TEntity, TItem>(TEntity entity) where TItem : IConfigurationItem, new()
		{
			var configuration = GetEntityConfiguration<TEntity>();

			return configuration.GetItems<TItem>(entity);
		}

		private ConditionalEntityConfiguration GetEntityConfiguration(Type ofEntity)
		{
			return (ConditionalEntityConfiguration)_entityConfigs.GetOrAdd(ofEntity, _ =>
			{
				var configurationType = typeof(ConditionalEntityConfiguration<>).MakeGenericType(ofEntity);

				var configuration = Activator.CreateInstance(configurationType);

				return configuration;
			});
		}

		private ConditionalEntityConfiguration<TEntity> GetEntityConfiguration<TEntity>()
		{
			return (ConditionalEntityConfiguration<TEntity>)GetEntityConfiguration(typeof(TEntity));
		}

		private abstract class ConditionalEntityConfiguration
		{
			protected readonly List<ItemInfoList> Items = new();

			public IEnumerable<TItem> GetItems<TItem>(object entity) where TItem : IConfigurationItem, new()
			{
				return Items.SelectMany(container => container.GetItems<TItem>(entity)).Cast<TItem>();
			}
		}

		private class ConditionalEntityConfiguration<TEntity> : ConditionalEntityConfiguration, IConditionalEntityConfiguration<TEntity>
		{
			public IEntityConfiguration<TEntity> When(Predicate<TEntity> condition)
			{
				var items = new List<ItemInfo<TEntity>>();

				Items.Add(new ItemInfoList<TEntity>(condition, items));

				return new EntityConfiguration<TEntity>(items);
			}

			public IEntityConfiguration<TEntity> Add<TItem>(Action<TEntity, TItem> init) where TItem : IConfigurationItem, new()
			{
				Items.Add(new ItemInfoList<TEntity>(_ => true, new[] { new ItemInfo<TEntity, TItem>(init) }));

				return this;
			}

			public IEnumerable<TItem> GetItems<TItem>(TEntity entity) where TItem : IConfigurationItem, new()
			{
				return base.GetItems<TItem>(entity);
			}
		}

		private class EntityConfiguration<TEntity> : IEntityConfiguration<TEntity>
		{
			private readonly ICollection<ItemInfo<TEntity>> _items;

			public EntityConfiguration(ICollection<ItemInfo<TEntity>> items)
			{
				_items = items;
			}

			public IEntityConfiguration<TEntity> Add<TItem>(Action<TEntity, TItem> init) where TItem : IConfigurationItem, new()
			{
				_items.Add(new ItemInfo<TEntity, TItem>(init));

				return this;
			}
		}

		private abstract class ItemInfo<TEntity>
		{
			public abstract bool TryGetItem<TItem>(TEntity entity, out IConfigurationItem item);
		}

		private class ItemInfo<TEntity, TItem> : ItemInfo<TEntity> where TItem : IConfigurationItem, new()
		{
			private readonly Action<TEntity, TItem> _itemInit;

			public ItemInfo(Action<TEntity, TItem> itemInit)
			{
				_itemInit = itemInit;
			}

			public override bool TryGetItem<T>(TEntity entity, out IConfigurationItem item)
			{
				if (typeof(TItem).IsAssignableTo(typeof(T)))
				{
					var result = new TItem();

					_itemInit?.Invoke(entity, result);

					item = result;
					return true;
				}

				item = null;
				return false;
			}
		}

		private abstract class ItemInfoList
		{
			public abstract IEnumerable<IConfigurationItem> GetItems<TItem>(object entity);
		}

		private class ItemInfoList<TEntity> : ItemInfoList
		{
			private readonly Predicate<TEntity> _condition;
			private readonly ICollection<ItemInfo<TEntity>> _items;

			public ItemInfoList(Predicate<TEntity> condition, ICollection<ItemInfo<TEntity>> items)
			{
				_condition = condition;
				_items = items;
			}

			public override IEnumerable<IConfigurationItem> GetItems<TItem>(object entity)
			{
				return GetItems<TItem>((TEntity)entity);
			}

			public IEnumerable<IConfigurationItem> GetItems<TItem>(TEntity entity)
			{
				if (_condition?.Invoke(entity) == false) yield break;

				foreach (var itemInfo in _items)
				{
					if (itemInfo.TryGetItem<TItem>(entity, out var item))
					{
						yield return item;
					}
				}
			}
		}
	}
}
