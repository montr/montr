using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IContentService
	{
		/// <summary>
		/// Get menu by id. All configured menu items returned, authorization checks should be performed.
		/// </summary>
		/// <param name="menuId"></param>
		/// <returns></returns>
		Menu GetMenu(string menuId);
	}
}
