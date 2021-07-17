using System;
using System.Collections.Generic;

namespace Montr.Core.Services
{
	public interface IConfigurationManager
	{
		void Configure(Type ofEntity, Action<IEntityConfiguration> config);

		void Configure<TEntity>(Action<IEntityConfiguration<TEntity>> config);

		IEnumerable<IConfigurationItem> GetItems(Type ofEntity, Type ofItem, object entity);

		IEnumerable<T> GetItems<TEntity, T>(TEntity entity) where T : IConfigurationItem;
	}

	public interface IEntityConfiguration : IConditionalEntityConfiguration
	{
		IConditionalEntityConfiguration When(Predicate<object> condition);

		IEnumerable<IConfigurationItem> GetItems(Type ofItem, object entity);
	}

	public interface IConditionalEntityConfiguration
	{
		// IConditionalEntityConfiguration Add(IConfigurationItem item);
	}

	public interface IEntityConfiguration<out TEntity> : IEntityConfiguration, IConditionalEntityConfiguration<TEntity>
	{
		IConditionalEntityConfiguration<TEntity> When(Predicate<TEntity> condition);
	}

	public interface IConditionalEntityConfiguration<out TEntity> : IConditionalEntityConfiguration
	{
		IConditionalEntityConfiguration<TEntity> Add(IConfigurationItem item);
	}

	public interface IConfigurationItem
	{
		int DisplayOrder { get; }
	}
}
