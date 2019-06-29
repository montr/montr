using LinqToDB.Data;

namespace Montr.Data.Linq2Db
{
	public static class DbContextExtensions
	{
		public static T SelectSequenceNextValue<T>(this DataConnection db, string sequenceName)
		{
			return db.Execute<T>($"select nextval('{sequenceName}')");
		}
	}
}
