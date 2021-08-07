using System;
using System.Collections.Generic;

namespace Montr.Core.Services
{
	public interface IConfigurationManager
	{
		void Configure<TEntity>(Action<IConditionalEntityConfiguration<TEntity>> config);

		IEnumerable<T> GetItems<TEntity, T>(TEntity entity) where T : IConfigurationItem, new();
	}

	public interface IConditionalEntityConfiguration<out TEntity> : IEntityConfiguration<TEntity>
	{
		IEntityConfiguration<TEntity> When(Predicate<TEntity> condition);
	}

	public interface IEntityConfiguration<out TEntity>
	{
		IEntityConfiguration<TEntity> Add<T>(Action<TEntity, T> init = null) where T : IConfigurationItem, new();
	}

	public interface IConfigurationItem
	{
		string Permission { get; }

		int DisplayOrder { get; }
	}
}
