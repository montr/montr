using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.MasterData.Models;
using Montr.Metadata.Models;

namespace Montr.MasterData.Services
{
	public interface IClassifierTypeRegistrator
	{
		public Task<ApiResult> Register(ClassifierType item, ICollection<FieldMetadata> fields, CancellationToken cancellationToken);
	}
}
