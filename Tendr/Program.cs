using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Tendr
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var hostBuilder = WebHost
				.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSentry();

			var host = hostBuilder.Build();

			host.Run();
		}
	}
}
