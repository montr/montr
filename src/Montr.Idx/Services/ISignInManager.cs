using System.Threading.Tasks;
using Montr.Core.Models;

namespace Montr.Idx.Services
{
	public interface ISignInManager
	{
		Task<ApiResult> PasswordSignIn(string userName, string password, bool isPersistent, bool lockoutOnFailure);
	}
}
