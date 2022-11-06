using System.Transactions;

namespace Montr.Core.Services.Implementations
{
	public class TransactionScopeUnitOfWorkFactory : IUnitOfWorkFactory
	{
		public IUnitOfWork Create()
		{
			var scope = new TransactionScope(
				TransactionScopeOption.Required,
				new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
				TransactionScopeAsyncFlowOption.Enabled);

			return new UnitOfWork(scope);
		}

		private class UnitOfWork : IUnitOfWork
		{
			private readonly TransactionScope _scope;

			public UnitOfWork(TransactionScope scope)
			{
				_scope = scope;
			}

			public void Commit()
			{
				_scope.Complete();
			}

			public void Dispose()
			{
				_scope.Dispose();
			}
		}
	}
}
