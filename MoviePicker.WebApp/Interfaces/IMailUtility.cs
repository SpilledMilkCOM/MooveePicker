using SendGrid;
using SM.COMS.Models;
using System.Threading.Tasks;

namespace SM.COMS.Utilities.Interfaces
{
	public interface IMailUtility
	{
		void Send(MailModel model);

		Task<Response> Send(string from, string to, string subject, string body);
	}
}