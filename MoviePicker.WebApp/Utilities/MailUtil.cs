using SendGrid;
using SendGrid.Helpers.Mail;
using SM.COMS.Models;
using SM.COMS.Utilities.Interfaces;

namespace SM.COMS.Utilities
{
	public class MailUtil : IMailUtility
	{
		public const string DO_NOT_REPLY = "do-not-reply@MooVeePicker.com";

		private string _apiKey;

		public MailUtil(string apiKey)
		{
			_apiKey = apiKey;
		}

		public void Send(MailModel model)
		{
			if (model.CanSubmit)
			{
				Send(model.From, model.To, model.Subject, model.Message);
				model.Information = "Message sent.";
				model.CanSubmit = false;        // prevent from multiple sends.
			}
			else
			{
				model.Information = "Cannot send message.";
			}
		}

		/// <summary> Send Mail...
		/// </summary>
		/// <param name="from">If from is null, then use "do-not-reply"</param>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public async void Send(string from, string to, string subject, string body)
		{
#if DEBUG
			return;
#endif
			var client = new SendGridClient(_apiKey);
			var fromEmail = new EmailAddress(from ?? DO_NOT_REPLY);
			var toEmail = new EmailAddress(to);

			var mail = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, body, body);

			var response = await client.SendEmailAsync(mail);
		}
	}
}