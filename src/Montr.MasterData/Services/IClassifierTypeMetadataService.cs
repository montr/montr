using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierTypeMetadataService
	{
		Task<ICollection<FieldMetadata>> GetMetadata(ClassifierType type, CancellationToken cancellationToken);
	}
}
