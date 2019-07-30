using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace Host
{
	public class Program
	{
		public static void Main(string[] args)
		{
			System.Console.Title = typeof(Startup).Namespace;

			var hostBuilder = WebHost
				.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseSentry()
				.UseSerilog((context, configuration) =>
				{
					configuration
						.MinimumLevel.Debug()
						.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
						.MinimumLevel.Override("System", LogEventLevel.Warning)
						.MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
						.Enrich.FromLogContext()
						.WriteTo.File($"../../../.logs/{typeof(Startup).Namespace}-{context.HostingEnvironment.EnvironmentName}.log")
						.WriteTo.Console(outputTemplate: "{Timestamp:o} [{Level:w4}] {SourceContext} - {Message:lj}{NewLine}{Exception}");
				});

			var host = hostBuilder.Build();

			host.Run();
		}
	}
}
