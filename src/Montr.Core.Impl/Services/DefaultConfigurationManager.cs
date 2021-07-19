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

		public IEnumerable<IConfigurationItem> GetItems(Type ofEntity, Type ofItem, object entity)
		{
			var configuration = GetEntityConfiguration(ofEntity);

			return configuration.GetItems(ofItem, entity);
		}

		public IEnumerable<T> GetItems<TEntity, T>(TEntity entity) where T : IConfigurationItem
		{
			foreach (var item in GetItems(typeof(TEntity), typeof(T), entity))
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
			private readonly List<ItemContainer> _items = new();

			public IConditionalEntityConfiguration When(Predicate<object> condition)
			{
				var items = new List<IConfigurationItem>();

				_items.Add(new ConditionalItemContainer(condition, items));

				return new ConditionalEntityConfiguration<TEntity>(items);
			}

			public IConditionalEntityConfiguration<TEntity> When(Predicate<TEntity> condition)
			{
				return (IConditionalEntityConfiguration<TEntity>)When((object entity) => condition((TEntity)entity));
			}

			public IConditionalEntityConfiguration<TEntity> Add(IConfigurationItem item)
			{
				_items.Add(new ItemContainer(item));

				return this;
			}

			public IEnumerable<IConfigurationItem> GetItems(Type ofItem, object entity)
			{
				foreach (var container in _items)
				{
					if (container.MeetCondition(entity))
					{
						foreach (var item in container.EnumerateItems())
						{
							if (ofItem.IsInstanceOfType(item))
							{
								yield return item;
							}
						}
					}
				}
			}
		}

		private class ConditionalEntityConfiguration<TEntity> : IConditionalEntityConfiguration<TEntity>
		{
			private readonly ICollection<IConfigurationItem> _items;

			public ConditionalEntityConfiguration(ICollection<IConfigurationItem> items)
			{
				_items = items;
			}

			public IConditionalEntityConfiguration<TEntity> Add(IConfigurationItem item)
			{
				_items.Add(item);

				return this;
			}
		}

		private class ItemContainer
		{
			private readonly IConfigurationItem _item;

			protected ItemContainer()
			{
			}

			public ItemContainer(IConfigurationItem item)
			{
				_item = item;
			}

			public virtual bool MeetCondition(object entity)
			{
				return true;
			}

			public virtual IEnumerable<IConfigurationItem> EnumerateItems()
			{
				yield return _item;
			}
		}

		private class ConditionalItemContainer : ItemContainer
		{
			private readonly Predicate<object> _condition;
			private readonly ICollection<IConfigurationItem> _items;

			public ConditionalItemContainer(Predicate<object> condition, ICollection<IConfigurationItem> items)
			{
				_condition = condition;
				_items = items;
			}

			public override bool MeetCondition(object entity)
			{
				return _condition(entity);
			}

			public override IEnumerable<IConfigurationItem> EnumerateItems()
			{
				return _items;
			}
		}
	}
}
