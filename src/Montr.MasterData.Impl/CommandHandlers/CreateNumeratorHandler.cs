using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;

namespace Montr.MasterData.Impl.CommandHandlers
{
	public class CreateNumeratorHandler : IRequestHandler<CreateNumerator, Numerator>
	{
		public Task<Numerator> Handle(CreateNumerator request, CancellationToken cancellationToken)
		{
			var result = new Numerator();

			return Task.FromResult(result);
		}
	}
}
