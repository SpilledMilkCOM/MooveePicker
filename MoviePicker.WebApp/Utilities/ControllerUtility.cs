﻿using MoviePicker.WebApp.Interfaces;
using MoviePicker.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace MoviePicker.WebApp.Utilities
{
	public class ControllerUtility : IControllerUtility
	{
		public static bool HasHttpPrefix(string url)
		{
			return url != null && (url.StartsWith("http://") || url.StartsWith("https://"));
		}

		public decimal? GetRequestDecimal(HttpRequestBase request, string key)
		{
			decimal? result = null;
			decimal parsed = 0;

			if (decimal.TryParse(request.Params[key], out parsed))
			{
				result = parsed;
			}

			return result;
		}

		public List<decimal> GetRequestDecimalList(HttpRequestBase request, string key)
		{
			char[] listDelimiter = { ',' };
			var result = new List<decimal>();
			var parms = request.Params;

			if (parms[key] != null)
			{
				var paramList = parms[key].Split(listDelimiter);

				foreach (var paramValue in paramList)
				{
					decimal value = 0;

					if (decimal.TryParse(paramValue, out value))
					{
						result.Add(value);
					}
				}
			}

			return result;
		}

		public Guid GetRequestGuid(HttpRequestBase request, string key)
		{
			Guid result = Guid.Empty;
			var value = request.Params[key];

			if (value != null)
			{
				Guid.TryParse(value, out result);
			}

			return result;
		}

		public int? GetRequestInt(HttpRequestBase request, string key)
		{
			int? result = null;
			int parsed = 0;

			if (int.TryParse(request.Params[key], out parsed))
			{
				result = parsed;
			}

			return result;
		}

		public List<int> GetRequestIntList(HttpRequestBase request, string key)
		{
			char[] listDelimiter = { ',' };
			var result = new List<int>();
			var parms = request.Params;

			if (parms[key] != null)
			{
				var paramList = parms[key].Split(listDelimiter);

				foreach (var paramValue in paramList)
				{
					int value = 0;

					if (int.TryParse(paramValue, out value))
					{
						result.Add(value);
					}
				}
			}

			return result;
		}

		public string GetRequestString(HttpRequestBase request, string key)
		{
			return request.Params[key];
		}

		public static void SetOpenGraph(dynamic viewBag, HttpRequestBase request)
		{
			viewBag.OpenGraphUrl = request.Url;
		}

		/// <summary>
		/// Initialize the ViewBag with the Twitter Card "meta" tag info.
		/// </summary>
		/// <param name="viewBag">The view's ViewBag property</param>
		/// <param name="card">The card type defined by Twitter "summary", "app", "player"</param>
		/// <param name="title">The card title</param>
		/// <param name="description">The descriptive text for the card</param>
		/// <param name="imageUrl">The url of the image</param>
		/// <param name="imageUrlAlt">Descriptive text of the image for the visuall impared</param>
		public static void SetTwitterCard(dynamic viewBag, string card = null, string title = null, string description = null, string imageUrl = null, string imageUrlAlt = null, string tweetText = null)
		{
			if (viewBag != null)
			{
				viewBag.TwitterCard = card ?? "summary";
				viewBag.TwitterCreatorId = Constants.TWITTER_CREATOR_ID;
				viewBag.TwitterId = Constants.TWITTER_ID;
				viewBag.TwitterTitle = title ?? Constants.APPLICATION_NAME;
				viewBag.TwitterDescription = description ?? "Don't know where to start with the Fantasy Movie League? Take a look at the MooVee Picker to help you with your picks!";
				viewBag.TwitterTweetText = tweetText ?? viewBag.TwitterTitle;

				if (!string.IsNullOrEmpty(imageUrl))
				{
					if (imageUrl.Substring(0, 1) == "~")
					{
						imageUrl = imageUrl.Replace("~", Constants.WEBSITE_URL);
					}
					else if (!HasHttpPrefix(imageUrl))
					{
						imageUrl = $"{Constants.WEBSITE_URL}/{imageUrl}";
					}

					//imageUrl = HttpUtility.UrlEncode(imageUrl);			// Puts '+' in the spaces.
					//imageUrl = WebUtility.HtmlEncode(imageUrl);			// Deals with escaping < and >
					imageUrl = imageUrl.Replace(" ", "%20");
				}
				else
				{
					imageUrl = $"{Constants.WEBSITE_URL}/Images/MooveePickerCow512x512.png";
				}

				viewBag.TwitterImage = imageUrl;
				viewBag.TwitterImageAlt = imageUrlAlt ?? "Logo of a piece of movie film with a finger pointing at a frame.";
				viewBag.OpenGraphUrl = Constants.WEBSITE_URL;
				viewBag.OpenGraphSiteName = Constants.APPLICATION_NAME;
			}
		}
	}
}