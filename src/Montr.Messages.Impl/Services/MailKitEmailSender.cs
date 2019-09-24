using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Montr.Messages.Services;

namespace Montr.Messages.Impl.Services
{
	public class MailKitEmailSender : IEmailSender
	{
		private readonly IOptionsMonitor<EmailSenderOptions> _options;

		public MailKitEmailSender(IOptionsMonitor<EmailSenderOptions> options)
		{
			_options = options;
		}

		public async Task Send(string email, string subject, string text)
		{
			var options = _options.CurrentValue;

			var message = new MimeMessage();

			var to = options.TestMode ? options.TestAddress : email;

			message.From.Add(MailboxAddress.Parse(options.From));
			message.To.Add(MailboxAddress.Parse(to));
			message.Subject = subject;
			message.Body = new TextPart(TextFormat.Html) { Text = text };

			using (var client = new SmtpClient())
			{
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				await client.ConnectAsync(options.Host, options.Port,
					options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

				await client.AuthenticateAsync(options.UserName, options.Password);

				await client.SendAsync(message);
				await client.DisconnectAsync(true);
			}
		}
	}
}
