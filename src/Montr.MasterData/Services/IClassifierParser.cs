using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierParser
	{
		void Reset();

		Task Parse(Stream stream, CancellationToken cancellationToken);

		ParseResult GetResult();
	}
}
