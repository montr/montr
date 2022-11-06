using System;

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
}
