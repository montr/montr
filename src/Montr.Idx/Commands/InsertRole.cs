using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class InsertRole : IRequest<ApiResult>
	{
		public Role Item { get; set; }
	}
}
