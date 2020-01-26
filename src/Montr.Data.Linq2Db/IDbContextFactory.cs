namespace Montr.Data.Linq2Db
{
	public interface IDbContextFactory
	{
		DbContext Create(string connectionName = null);
	}

	public class DefaultDbContextFactory : IDbContextFactory
	{
		public DbContext Create(string connectionName = null)
		{
			return new DbContext(connectionName);
		}
	}
}
