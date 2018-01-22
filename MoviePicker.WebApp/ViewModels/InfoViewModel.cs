using MoviePicker.WebApp.Interfaces;

namespace MoviePicker.WebApp.Models
{
	public class InfoViewModel : IInfoViewModel
	{
		public IClientInfoModel ClientInfo { get; set; }

		public IServerInfoModel ServerInfo { get; set; }
	}
}