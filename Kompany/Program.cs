using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Kompany
{
	public class Program
	{
		public static void Main(string[] args)
		{
			System.Console.Title = typeof(Startup).Namespace;

			var hostBuilder = WebHost
				.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSentry();

			var host = hostBuilder.Build();

			host.Run();
		}
	}
}
