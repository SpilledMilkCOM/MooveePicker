using MoviePicker.WebApp.Interfaces;

namespace MoviePicker.WebApp.Models
{
	public class InfoViewModel : IInfoViewModel
	{
		public InfoViewModel(IClientInfoModel clientInfo, IServerInfoModel serverInfo)
		{
			ClientInfo = clientInfo;
			ServerInfo = serverInfo;
		}

		public IClientInfoModel ClientInfo { get; set; }

		public IServerInfoModel ServerInfo { get; set; }
	}
}