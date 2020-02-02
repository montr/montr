using System.Threading.Tasks;

namespace Montr.Messages.Services
{
	public interface IEmailSender
	{
		Task Send(string email, string subject, string message);
	}
}
