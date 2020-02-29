using SendGrid;
using SM.COMS.Models;
using System.Threading.Tasks;

namespace SM.COMS.Utilities.Interfaces
{
	public interface IMailUtility
	{
		Task<Response> SendAsync(MailModel model);

		Task<Response> SendAsync(string from, string to, string subject, string body);
	}
}