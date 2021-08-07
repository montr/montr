using System;
using System.Collections.Generic;

namespace Montr.Core.Services
{
	public interface IConfigurationManager
	{
		void Configure(Type ofEntity, Action<IEntityConfiguration> config);

		void Configure<TEntity>(Action<IEntityConfiguration<TEntity>> config);

		IEnumerable<T> GetItems<TEntity, T>(TEntity entity) where T : IConfigurationItem;
	}

	public interface IEntityConfiguration : IConditionalEntityConfiguration
	{
		// todo: remove from interface (move to concrete) ???
		IEnumerable<IConfigurationItem> GetItems<T>(object entity);
	}

	public interface IConditionalEntityConfiguration
	{
	}

	public interface IEntityConfiguration<out TEntity> : IEntityConfiguration, IConditionalEntityConfiguration<TEntity>
	{
		IConditionalEntityConfiguration<TEntity> When(Predicate<TEntity> condition);
	}

	public interface IConditionalEntityConfiguration<out TEntity> : IConditionalEntityConfiguration
	{
		IConditionalEntityConfiguration<TEntity> Add<T>(Action<TEntity, T> init = null) where T : IConfigurationItem, new();
	}

	public interface IConfigurationItem
	{
		string Permission { get; }

		int DisplayOrder { get; }
	}
}
