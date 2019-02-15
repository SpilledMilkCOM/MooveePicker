namespace MoviePicker.WebApp.Models
{
	public class Constants
	{
		public const string APPLICATION_NAME = "MooVee Picker";
		public const string APPLICATION_NAME_ABBR = "MooV";
		public const string COMPANY_NAME = "Spilled Milk";

		public const string TWITTER_ID = "@SpilledMilkCOM";
		public const string TWITTER_CREATOR_ID = "@SpilledMilkCOM";

		public const string WEBSITE_URL = "http://MooveePicker.com";

		public static string GoogleAdCode
		{
			get
			{
				return @"
<script async src=""//pagead2.googlesyndication.com/pagead/js/adsbygoogle.js""></script>
<script>
	(adsbygoogle = window.adsbygoogle || []).push({
				google_ad_client: ""ca-pub-8791392312556224"",
		enable_page_level_ads: true
	});
</script>";
			}
		}
	}
}