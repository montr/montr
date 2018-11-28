using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Idx.Areas.Identity.IdentityHostingStartup))]
namespace Idx.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}