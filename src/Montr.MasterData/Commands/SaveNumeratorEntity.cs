using MediatR;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Commands
{
	public class SaveNumeratorEntity : IRequest<ApiResult>
	{
		public NumeratorEntity Item { get; set; }
	}
}
