using MediatR;
using Montr.Core.Models;

namespace Montr.Core.Queries
{
	public class GetMenu : IRequest<Menu>
	{
		public string MenuId { get; set; }
	}
}
