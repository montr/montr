using System.Threading.Tasks;
using Dokumento.Models;

namespace Dokumento.Services
{
	public interface IDocumentRepository
	{
		Task Create(Document document);
	}
}
