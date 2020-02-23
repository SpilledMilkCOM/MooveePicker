using SM.COMS.Models.Interfaces;

namespace SM.COMS.Utilities.Interfaces
{
	public interface ISendMail
	{
		void Send(IMailModel model);
	}
}