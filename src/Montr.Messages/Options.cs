using System.ComponentModel.DataAnnotations;

namespace Montr.Messages
{
	public class Options
	{
		[Required]
		public string From { get; set; }

		[Required]
		public string Host { get; set; }

		public int Port { get; set; }

		public bool UseSsl { get; set; } = true;

		[Required]
		public string UserName { get; set; }

		[Required]
		public string Password { get; set; }

		public bool TestMode { get; set; }

		public string TestAddress { get; set; }
	}
}
