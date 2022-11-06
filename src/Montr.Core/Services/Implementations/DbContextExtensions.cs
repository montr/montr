using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;

namespace Montr.Core.Services.Implementations
{
	public static class DbContextExtensions
	{
		public static T SelectSequenceNextValue<T>(this DataConnection db, string sequenceName)
		{
			return db.Execute<T>($"select nextval('{sequenceName}')");
		}

		public static async Task<bool> HasData<TTable>(this IDbContextFactory dbContextFactory, CancellationToken cancellationToken) where TTable : class
		{
			using (var db = dbContextFactory.Create())
			{
				return await db.GetTable<TTable>().Take(1).CountAsync(cancellationToken) > 0;
			}
		}
	}
}
