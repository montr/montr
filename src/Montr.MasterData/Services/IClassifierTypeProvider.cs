using System.Threading;
using System.Threading.Tasks;
using Montr.Data.Linq2Db;
using Montr.MasterData.Models;

namespace Montr.MasterData.Services
{
	/// <summary>
	/// Provides specific information about some classifier types, e.g. numerators.
	/// </summary>
	public interface IClassifierTypeProvider
	{
		Task Insert(DbContext db, ClassifierType type, Classifier item, CancellationToken cancellationToken);
	}
}
