using MoviePicker.WebApp.Interfaces;

namespace MoviePicker.WebApp.ViewModels
{
	public class InfoViewModel : IInfoViewModel
	{
		public InfoViewModel(IClientInfoModel clientInfo, IMinerModel minerModel, IServerInfoModel serverInfo)
		{
			ClientInfo = clientInfo;
			MinerModel = minerModel;
			ServerInfo = serverInfo;
		}

		public IClientInfoModel ClientInfo { get; private set; }

		public IMinerModel MinerModel { get; private set; }

		public IServerInfoModel ServerInfo { get; private set; }
	}
}