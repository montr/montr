using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IContentService
	{
		void Rebuild();

		Menu GetMenu(string menuId);
	}
}
