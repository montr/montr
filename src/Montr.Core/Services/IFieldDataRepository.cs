using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Core.Services
{
	public interface IFieldDataRepository
	{
		Task Insert(string entityTypeCode, Guid entityUid, FieldData data, CancellationToken cancellationToken);
	}
}
