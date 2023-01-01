using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IFieldDesigner
	{
		Task<FieldMetadata> GetMetadata(PropertyInfo property, CancellationToken cancellationToken);
	}
}
