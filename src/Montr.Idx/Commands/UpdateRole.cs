using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class UpdateRole : IRequest<ApiResult>
	{
		public Role Item { get; set; }
	}
}
