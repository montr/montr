namespace Tendr.Entities
{
    public class DbContext : LinqToDB.Data.DataConnection
	{
		public DbContext() : base("Default")
		{
		}
	}
}