using Microsoft.AspNetCore.Builder;
using Montr.Modularity;

namespace Montr.Web
{
	public interface IWebModule : IModule
	{
		void Configure(IApplicationBuilder app);
	}
}
