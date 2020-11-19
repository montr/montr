using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	/// <summary>
	/// Provides specific information about some classifier types, e.g. numerators.
	/// </summary>
	public interface IClassifierTypeProvider
	{
		Type ClassifierType { get; }

		Task<SearchResult<Classifier>> Search(ClassifierType type,
			ClassifierSearchRequest request, CancellationToken cancellationToken);

		/// <summary>
		/// Create classifier item with defaults to display to user before inserting to database.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<Classifier> Create(ClassifierType type, CancellationToken cancellationToken);

		Task Insert(ClassifierType type, Classifier item, CancellationToken cancellationToken);

		Task Update(ClassifierType type, Classifier item, CancellationToken cancellationToken);
	}
}
