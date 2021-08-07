using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Montr.Core.Services;

namespace Montr.Core.Impl.Services
{
	public class DefaultConfigurationManager : IConfigurationManager
	{
		private readonly ConcurrentDictionary<Type, IEntityConfiguration> _entityConfigs = new();

		public void Configure(Type ofEntity, Action<IEntityConfiguration> config)
		{
			var configuration = GetEntityConfiguration(ofEntity);

			config?.Invoke(configuration);
		}

		public void Configure<TEntity>(Action<IEntityConfiguration<TEntity>> config)
		{
			Configure(typeof(TEntity), configuration => config((IEntityConfiguration<TEntity>)configuration));
		}

		public IEnumerable<T> GetItems<TEntity, T>(TEntity entity) where T : IConfigurationItem
		{
			var configuration = GetEntityConfiguration(typeof(TEntity));

			var items = configuration.GetItems<T>(entity);

			foreach (var item in items)
			{
				yield return (T)item;
			}
		}

		private IEntityConfiguration GetEntityConfiguration(Type ofEntity)
		{
			return _entityConfigs.GetOrAdd(ofEntity, _ =>
			{
				var configurationType = typeof(EntityConfiguration<>).MakeGenericType(ofEntity);

				return (IEntityConfiguration)Activator.CreateInstance(configurationType);
			});
		}

		private class EntityConfiguration<TEntity> : IEntityConfiguration<TEntity>
		{
			private readonly List<ConfigurationItem<TEntity>> _items = new();

			public IConditionalEntityConfiguration<TEntity> When(Predicate<TEntity> condition)
			{
				var items = new List<ItemInfo<TEntity>>();

				_items.Add(new ConditionalConfigurationItem<TEntity>(condition, items));

				return new ConditionalEntityConfiguration<TEntity>(items); // todo: pass conditional item (?)
			}

			public IConditionalEntityConfiguration<TEntity> Add<T>(Action<TEntity, T> init) where T : IConfigurationItem, new()
			{
				_items.Add(new ConfigurationItem<TEntity>(new ItemInfo<TEntity, T>(init)));

				return this;
			}

			public IEnumerable<IConfigurationItem> GetItems<T>(object entity)
			{
				foreach (var container in _items)
				{
					// todo: check condition in EnumerateItems
					if (container.MeetCondition((TEntity)entity))
					{
						foreach (var item in container.EnumerateItems(typeof(T), (TEntity)entity))
						{
							yield return item;
						}
					}
				}
			}
		}

		private class ConditionalEntityConfiguration<TEntity> : IConditionalEntityConfiguration<TEntity>
		{
			private readonly ICollection<ItemInfo<TEntity>> _items;

			public ConditionalEntityConfiguration(ICollection<ItemInfo<TEntity>> items)
			{
				_items = items;
			}

			public IConditionalEntityConfiguration<TEntity> Add<T>(Action<TEntity, T> init) where T : IConfigurationItem, new()
			{
				_items.Add(new ItemInfo<TEntity, T>(init));

				return this;
			}
		}

		private abstract class ItemInfo<TEntity>
		{
			public abstract bool TryGetItem(Type ofItem, TEntity entity, out IConfigurationItem item);
		}

		private class ItemInfo<TEntity, TITem> : ItemInfo<TEntity> where TITem : IConfigurationItem, new()
		{
			private readonly Action<TEntity, TITem> _itemInit;

			public ItemInfo(Action<TEntity, TITem> itemInit)
			{
				_itemInit = itemInit;
			}

			public override bool TryGetItem(Type ofItem, TEntity entity, out IConfigurationItem item)
			{
				if (/*ofItem == typeof(TITem) ||*/ ofItem.IsAssignableTo(typeof(TITem)))
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

		// todo: rename to list
		private class ConfigurationItem<TEntity>
		{
			private readonly ItemInfo<TEntity> _item;

			protected ConfigurationItem()
			{
			}

			public ConfigurationItem(ItemInfo<TEntity> item)
			{
				_item = item;
			}

			public virtual bool MeetCondition(TEntity entity)
			{
				return true;
			}

			public virtual IEnumerable<IConfigurationItem> EnumerateItems(Type ofItem, TEntity entity)
			{
				if (_item.TryGetItem(ofItem, entity, out var item))
				{
					yield return item;
				}
			}
		}

		private class ConditionalConfigurationItem<TEntity> : ConfigurationItem<TEntity>
		{
			private readonly Predicate<TEntity> _condition;
			private readonly ICollection<ItemInfo<TEntity>> _items;

			public ConditionalConfigurationItem(Predicate<TEntity> condition, ICollection<ItemInfo<TEntity>> items)
			{
				_condition = condition;
				_items = items;
			}

			public override bool MeetCondition(TEntity entity)
			{
				return _condition(entity);
			}

			public override IEnumerable<IConfigurationItem> EnumerateItems(Type ofItem, TEntity entity)
			{
				foreach (var itemInfo in _items)
				{
					if (itemInfo.TryGetItem(ofItem, entity, out var item))
					{
						yield return item;
					}
				}
			}
		}
	}
}
