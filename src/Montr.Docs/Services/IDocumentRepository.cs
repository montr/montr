using System.Threading.Tasks;
using Montr.Docs.Models;

namespace Montr.Docs.Services
{
	public interface IDocumentRepository
	{
		Task Create(Document document);
	}
}
