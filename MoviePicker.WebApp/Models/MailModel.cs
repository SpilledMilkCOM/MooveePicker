using SM.COMS.Models.Interfaces;
using System;

namespace SM.COMS.Models
{
	public class MailModel : IMailModel
	{
		public bool CanSubmit { get; set; }

		public string From { get; set; }

		/// <summary> Display on the form as informational feedback.
		/// </summary>
		public string Information { get; set; }

		public Guid ItemId { get; set; }

		public string Message { get; set; }

		public string Subject { get; set; }

		public string To { get; set; }
	}
}