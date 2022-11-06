using Microsoft.EntityFrameworkCore;

namespace Montr.Idx.Entities
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{
		}
	}
}
