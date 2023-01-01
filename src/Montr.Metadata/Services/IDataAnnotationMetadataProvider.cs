using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Montr.Metadata.Models;

namespace Montr.Metadata.Services
{
	public interface IDataAnnotationMetadataProvider
	{
		Task<ICollection<FieldMetadata>> GetMetadata(Type type, CancellationToken cancellationToken);
	}
}
