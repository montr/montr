namespace Montr.Core
{
	public class MigrationOptions
	{
		/// <summary>
		/// Path where .sql migration files to be run are located.
		/// </summary>
		public string MigrationPath { get; set; }

		/// <summary>
		/// Forces migration of all sql files that were previously migrated
		/// </summary>
		public bool Force { get; set; }
	}
}
