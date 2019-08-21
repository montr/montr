using System;
using Idx.Entities;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Identity;

namespace Idx.Services
{
	public class DbConnectionFactory : IConnectionFactory
	{
		public IDataContext GetContext()
		{
			return new Montr.Data.Linq2Db.DbContext();
		}

		public DataConnection GetConnection()
		{
			return new IdentityDataConnection<DbUser, DbRole, Guid>();
		}
	}
}
