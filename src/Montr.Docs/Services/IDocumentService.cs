using System.Threading;
using System.Threading.Tasks;
using Montr.Docs.Models;

namespace Montr.Docs.Services
{
	public interface IDocumentService
	{
		Task Create(Document document, CancellationToken cancellationToken);
	}
}
