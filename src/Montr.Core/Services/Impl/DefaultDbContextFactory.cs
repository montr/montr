namespace Montr.Core.Services.Impl
{
	public class DefaultDbContextFactory : IDbContextFactory
	{
		public DbContext Create(string connectionName = null)
		{
			return new DbContext(connectionName);
		}
	}
}
