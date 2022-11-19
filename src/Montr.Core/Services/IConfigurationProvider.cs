using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	/// <summary>
	/// Returns items, configured in <see cref="IConfigurationRegistry"/> with authorisation checks.
	/// </summary>
	public interface IConfigurationProvider
	{
		Task<ICollection<TItem>> GetItems<TItem>(Type ofEntity, object entity, ClaimsPrincipal principal) where TItem : IConfigurationItem, new();

		Task<ICollection<TItem>> GetItems<TEntity, TItem>(TEntity entity, ClaimsPrincipal principal) where TItem : IConfigurationItem, new();
	}
}
