using MediatR;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Commands
{
	public class InsertUser : IRequest<ApiResult>
	{
		public User Item { get; set; }
	}
}
