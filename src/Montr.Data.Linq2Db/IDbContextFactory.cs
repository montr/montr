namespace Montr.Data.Linq2Db
{
	public interface IDbContextFactory
	{
		DbContext Create(string configurationString = null);
	}

	public class DefaultDbContextFactory : IDbContextFactory
	{
		public DbContext Create(string configurationString = null)
		{
			return new DbContext(configurationString);
		}
	}
}
