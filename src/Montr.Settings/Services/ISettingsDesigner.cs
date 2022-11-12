using System.Reflection;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Settings.Services
{
	public interface ISettingsDesigner
	{
		Task<FieldMetadata> GetMetadata(PropertyInfo property);
	}
}
