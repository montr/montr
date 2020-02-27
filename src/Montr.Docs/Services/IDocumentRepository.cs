using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Services;
using Montr.Docs.Models;

namespace Montr.Docs.Services
{
	public interface IDocumentRepository : IRepository<Document>
	{
		Task Create(Document document, CancellationToken cancellationToken);
	}
}
