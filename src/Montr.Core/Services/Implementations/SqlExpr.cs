using LinqToDB;

namespace Montr.Core.Services.Implementations
{
	public static class SqlExpr
	{
		/// <summary>
		/// Case insensitive version of like.</summary>
		/// <param name="matchExpression"></param>
		/// <param name="pattern"></param>
		/// <returns></returns>
		/// <see cref="http://github.com/linq2db/linq2db/tree/master/Source/LinqToDB/Sql/"/>
		/// <see cref="http://www.postgresql.org/docs/current/functions-matching.html#FUNCTIONS-LIKE"/>
		[Sql.Expression("{0} ilike {1}", PreferServerSide = true)]
		// ReSharper disable once InconsistentNaming
		public static bool ILike(string matchExpression, string pattern)
		{
			return matchExpression.Contains(pattern);
		}
	}
}
