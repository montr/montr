using Microsoft.EntityFrameworkCore;

namespace Montr.Idx.Impl.Entities
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}
	}
}
