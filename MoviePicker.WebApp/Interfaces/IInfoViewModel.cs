using MoviePicker.WebApp.Interfaces;

namespace MoviePicker.WebApp.Interfaces
{
	public interface IInfoViewModel
	{
		IClientInfoModel ClientInfo { get; set; }

		IServerInfoModel ServerInfo { get; set; }
	}
}