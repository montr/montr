using System;
using System.Threading.Tasks;

namespace Montr.Kompany.Services
{
	public interface ICurrentCompanyProvider
	{
		Task<Guid> GetCompanyUid();
	}
}
