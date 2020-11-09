using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Montr.Data.Linq2Db;
using Montr.MasterData.Impl.Entities;
using Montr.MasterData.Models;
using Montr.MasterData.Services;

namespace Montr.MasterData.Impl.Services
{
	public class NumeratorTypeProvider : IClassifierTypeProvider
	{
		public static readonly string TypeCode = nameof(Numerator);

		public async Task Insert(DbContext db, ClassifierType type, Classifier item, CancellationToken cancellationToken)
		{
			var numerator = (Numerator)item;

			await db.GetTable<DbNumerator>()
				.Value(x => x.Uid, item.Uid)
				.Value(x => x.EntityTypeCode, numerator.EntityTypeCode ?? "DocumentType")
				// .Value(x => x.Name, numerator.Name)
				.Value(x => x.Pattern, numerator.Pattern)
				.Value(x => x.Periodicity, numerator.Periodicity.ToString())
				.Value(x => x.IsActive, numerator.IsActive)
				.Value(x => x.IsSystem, numerator.IsSystem)
				.InsertAsync(cancellationToken);
		}
	}
}
