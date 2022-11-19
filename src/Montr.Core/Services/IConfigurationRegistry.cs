using System;
using System.Collections.Generic;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IConfigurationRegistry
	{
		void Configure<TEntity>(Action<IConditionalEntityConfiguration<TEntity>> config);

		IEnumerable<TItem> GetItems<TItem>(Type ofEntity, object entity) where TItem : IConfigurationItem, new();

		IEnumerable<TItem> GetItems<TEntity, TItem>(TEntity entity) where TItem : IConfigurationItem, new();
	}

	public interface IConditionalEntityConfiguration<out TEntity> : IEntityConfiguration<TEntity>
	{
		IEntityConfiguration<TEntity> When(Predicate<TEntity> condition);
	}

	public interface IEntityConfiguration<out TEntity>
	{
		IEntityConfiguration<TEntity> Add<TItem>(Action<TEntity, TItem> init = null) where TItem : IConfigurationItem, new();
	}
}
