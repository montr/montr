using System;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Identity;

namespace Idx.Entities
{
	public class DbConnectionFactory : IConnectionFactory
	{
		public IDataContext GetContext()
		{
			return new DbContext();
		}

		public DataConnection GetConnection()
		{
			return new IdentityDataConnection<DbUser, DbRole, Guid>();
		}
	}
}