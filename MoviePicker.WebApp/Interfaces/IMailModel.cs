using System;

namespace SM.COMS.Models.Interfaces
{
	public interface IMailModel
	{
		bool CanSubmit { get; set; }
		string From { get; set; }
		string Information { get; set; }
		Guid ItemId { get; set; }
		string Message { get; set; }
		string Subject { get; set; }
		string To { get; set; }
	}
}