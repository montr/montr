namespace Montr.Core.Services
{
	public interface IDbContextFactory
	{
		DbContext Create(string connectionName = null);
	}

	public class DbContext : LinqToDB.Data.DataConnection
	{
		public DbContext()
		{
		}

		public DbContext(string connectionName) : base(connectionName)
		{
		}
	}
}
