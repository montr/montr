using System.Threading.Tasks;
using Montr.Dokumento.Models;

namespace Montr.Dokumento.Services
{
	public interface IDocumentRepository
	{
		Task Create(Document document);
	}
}
