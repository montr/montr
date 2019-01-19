using LinqToDB.Data;
using LinqToDB.DataProvider.PostgreSQL;

namespace Kompany.Tests
{
	public class TestHelper
	{
		public static void InitDb()
		{
			DataConnection
				.AddConfiguration(
					"Default",
					"Server=localhost;Port=5434;Database=kompany-dev;User Id=web;Password=secret;",
					new PostgreSQLDataProvider("Default", PostgreSQLVersion.v95));
			DataConnection.DefaultConfiguration = "Default";
		}
	}
}
