using SM.COMS.Models;

namespace SM.COMS.Utilities.Interfaces
{
	public interface IMailUtility
	{
		void Send(MailModel model);
		void Send(string from, string to, string subject, string body);
	}
}