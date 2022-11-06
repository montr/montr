using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IMetadataProvider
	{
		Task<DataView> GetView(string viewId);
	}
}
