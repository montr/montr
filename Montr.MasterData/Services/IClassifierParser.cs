using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierParser
	{
		Task<ParseResult> Parse(Stream stream, CancellationToken cancellationToken);
	}
}
