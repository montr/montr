using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IContentService
	{
		/// <summary>
		/// Rebuild content (menu items) from all registered <see cref="IContentProvider"/>
		/// </summary>
		void Rebuild();

		Menu GetMenu(string menuId);
	}
}
