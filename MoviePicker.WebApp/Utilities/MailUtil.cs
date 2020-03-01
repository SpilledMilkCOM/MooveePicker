using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using SendGrid;
using SendGrid.Helpers.Mail;
using SM.COMS.Models;
using SM.COMS.Utilities.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.COMS.Utilities
{
	public class MailUtil : IMailUtility
	{
		public const string DO_NOT_REPLY = "do-not-reply@MooVeePicker.com";

		private string _apiKey;
		private string _defaulFromEmail;
		private string _defaultToEmail;
		private TelemetryClient _telemetryClient;

		public MailUtil(string apiKey, string defaultToEmail, TelemetryClient telemetryClient)
		{
			_apiKey = apiKey;
			_defaulFromEmail = DO_NOT_REPLY;        // Inject this "later"
			_defaultToEmail = defaultToEmail;
			_telemetryClient = telemetryClient;
		}

		public Task<Response> SendAsync(MailModel model)
		{
			Task<Response> result = null;

			if (model.CanSubmit)
			{
				result = SendAsync(model.From, model.To, model.Subject, model.Message);
				model.Information = "Message sent.";
				model.CanSubmit = false;        // prevent from multiple sends.
			}
			else
			{
				model.Information = "Cannot send message.";
			}

			return result;
		}

		/// <summary> Send Mail...
		/// </summary>
		/// <param name="from">If from is null, then use "do-not-reply"</param>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public async Task<Response> SendAsync(string from, string to, string subject, string body)
		{
			//#if DEBUG
			//			return;
			//#endif

			_telemetryClient.TrackTrace("Sending Email: ", SeverityLevel.Information
				, new Dictionary<string, string> { { "from", from }, { "to", to }, { "subject", subject }, { "body", body } });

			var client = new SendGridClient(_apiKey);
			var fromEmail = new EmailAddress(from ?? _defaulFromEmail);
			var toEmail = new EmailAddress(to ?? _defaultToEmail);

			var mail = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, body, null);

			return await client.SendEmailAsync(mail);
		}
	}
}