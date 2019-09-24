using System.ComponentModel;
using System.Threading.Tasks;

namespace Montr.Messages.Services
{
	public interface IEmailSender
	{
		Task Send(string email, string subject, string message);
	}

	public class EmailSenderOptions
	{
		public const string SectionName = "EmailSender";

		public string From { get; set; }

		public string Host { get; set; }

		public int Port { get; set; }

		// [DefaultValue(true)]
		public bool UseSsl { get; set; } = true;

		public string UserName { get; set; }

		public string Password { get; set; }

		public bool TestMode { get; set; }

		public string TestAddress { get; set; }
	}
}
