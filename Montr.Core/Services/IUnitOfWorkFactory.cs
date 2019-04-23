using System;
using System.Transactions;

namespace Montr.Core.Services
{
	public interface IUnitOfWork : IDisposable
	{
		void Commit();
	}

	public interface IUnitOfWorkFactory
	{
		IUnitOfWork Create();
	}

	public class TransactionScopeUnitOfWorkFactory : IUnitOfWorkFactory
	{
		// todo: remove
		public bool Commitable { get; set; } = true;

		public IUnitOfWork Create()
		{
			var scope = new TransactionScope(
				TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
				TransactionScopeAsyncFlowOption.Enabled);

			return new UnitOfWork(scope, Commitable);
		}

		private class UnitOfWork : IUnitOfWork
		{
			private readonly TransactionScope _scope;
			private readonly bool _commitable;

			public UnitOfWork(TransactionScope scope, bool commitable)
			{
				_scope = scope;
				_commitable = commitable;
			}
			
			public void Commit()
			{
				if (_commitable)
				{
					_scope.Complete();
				}
			}

			public void Dispose()
			{
				_scope.Dispose();
			}
		}
	}
}
