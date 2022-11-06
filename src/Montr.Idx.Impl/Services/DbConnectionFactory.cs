using System;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Identity;
using Montr.Core.Services;
using Montr.Idx.Impl.Entities;

namespace Montr.Idx.Impl.Services
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
