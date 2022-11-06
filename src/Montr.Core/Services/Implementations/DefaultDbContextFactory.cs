namespace Montr.Core.Services.Implementations
{
	public class DefaultDbContextFactory : IDbContextFactory
	{
		public DbContext Create(string connectionName = null)
		{
			return new DbContext(connectionName);
		}
	}
}
