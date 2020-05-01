using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class InsertNumerator : IRequest<ApiResult>
	{
		public Numerator Item { get; set; }
	}
}
