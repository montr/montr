using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	/// <summary>
	/// Provides specific information about classifier types, e.g. numerators.
	/// </summary>
	public interface IClassifierRepository
	{
		Type ClassifierType { get; }

		Task<SearchResult<Classifier>> Search(ClassifierSearchRequest request, CancellationToken cancellationToken);

		/// <summary>
		/// Create classifier item with defaults to display to user before inserting to database.
		/// </summary>
		Task<Classifier> Create(ClassifierCreateRequest request, CancellationToken cancellationToken);

		Task<ApiResult> Insert(Classifier item, CancellationToken cancellationToken);

		Task<ApiResult> Update(Classifier item, CancellationToken cancellationToken);
	}
}
