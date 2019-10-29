using Microsoft.AspNetCore.Builder;

namespace Montr.Core
{
	public interface IWebModule : IModule
	{
		void Configure(IApplicationBuilder app);
	}
}
