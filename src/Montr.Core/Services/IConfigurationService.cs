using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	/// <summary>
	/// Returns items, configured in <see cref="IConfigurationManager"/>  with authorisation checks.
	/// </summary>
	public interface IConfigurationService
	{
		Task<ICollection<T>> GetItems<TEntity, T>(TEntity entity, ClaimsPrincipal principal) where T : IConfigurationItem, new();
	}
}
