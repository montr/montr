using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Montr.Messages.Services.Implementations
{
	public class MailKitEmailSender : IEmailSender
	{
		private readonly IOptionsMonitor<Options> _options;

		public MailKitEmailSender(IOptionsMonitor<Options> options)
		{
			_options = options;
		}

		public async Task Send(string email, string subject, string text, CancellationToken cancellationToken)
		{
			var options = _options.CurrentValue;

			var message = new MimeMessage();

			string to;

			if (options.TestMode)
			{
				to = options.TestAddress;

				message.Headers.Add("X-Montr-TestMode", "true");
				message.Headers.Add("X-Montr-OriginalTo", email);
			}
			else
			{
				to = email;
			}

			message.From.Add(MailboxAddress.Parse(options.From));
			message.To.Add(MailboxAddress.Parse(to));
			message.Subject = subject;
			message.Body = new TextPart(TextFormat.Html) { Text = text };

			using (var client = new SmtpClient())
			{
				// For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				await client.ConnectAsync(options.Host, options.Port,
					options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, cancellationToken);

				await client.AuthenticateAsync(options.UserName, options.Password, cancellationToken);

				await client.SendAsync(message, cancellationToken);
				await client.DisconnectAsync(true, cancellationToken);
			}
		}
	}
}
