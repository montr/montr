using System.Threading;
using System.Threading.Tasks;
using Montr.Core.Models;
using Montr.Idx.Models;

namespace Montr.Idx.Services
{
	public interface IUserManager
	{
		Task<ApiResult> CreateUser(User user, string optionalPassword, CancellationToken cancellationToken);
	}
}
