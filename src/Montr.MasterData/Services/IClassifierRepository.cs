using System;
using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.MasterData.Commands;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	/// <summary>
	/// Provides specific information about classifier types, e.g. numerators.
	/// </summary>
	public interface IClassifierRepository
	{
		Type ClassifierType { get; }

		Task<Classifier> Get(string typeCode, Guid uid, CancellationToken cancellationToken = default);

		Task<Classifier> Get(string typeCode, string code, CancellationToken cancellationToken = default);

		Task<SearchResult<Classifier>> Search(ClassifierSearchRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create classifier item with defaults to display to user before inserting to database.
		/// </summary>
		Task<Classifier> Create(ClassifierCreateRequest request, CancellationToken cancellationToken = default);

		Task<ApiResult> Insert(Classifier item, CancellationToken cancellationToken = default);

		Task<ApiResult> Update(Classifier item, CancellationToken cancellationToken = default);

		Task<ApiResult> Delete(DeleteClassifier request, CancellationToken cancellationToken = default);
	}
}
