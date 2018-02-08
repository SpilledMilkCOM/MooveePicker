namespace MoviePicker.WebApp.Interfaces
{
	public interface IInfoViewModel
	{
		IClientInfoModel ClientInfo { get; }

		IMinerModel MinerModel { get; }

		IServerInfoModel ServerInfo { get; }
	}
}