﻿using SendGrid;
using SendGrid.Helpers.Mail;
using SM.COMS.Models;
using SM.COMS.Utilities.Interfaces;
using System.Threading.Tasks;

namespace SM.COMS.Utilities
{
	public class MailUtil : IMailUtility
	{
		public const string DO_NOT_REPLY = "do-not-reply@MooVeePicker.com";

		private string _apiKey;
		private string _defaulFromEmail;
		private string _defaultToEmail;

		public MailUtil(string apiKey, string defaultToEmail)
		{
			_apiKey = apiKey;
			_defaulFromEmail = DO_NOT_REPLY;		// Inject this "later"
			_defaultToEmail = defaultToEmail;
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
		public async Task<Response> Send(string from, string to, string subject, string body)
		{
//#if DEBUG
//			return;
//#endif
			var client = new SendGridClient(_apiKey);
			var fromEmail = new EmailAddress(from ?? _defaulFromEmail);
			var toEmail = new EmailAddress(to ?? _defaultToEmail);

			var mail = MailHelper.CreateSingleEmail(fromEmail, toEmail, subject, body, null);

			return await client.SendEmailAsync(mail);
		}
	}
}