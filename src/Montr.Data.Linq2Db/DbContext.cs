namespace Montr.Data.Linq2Db
{
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
