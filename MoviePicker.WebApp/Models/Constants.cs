namespace MoviePicker.WebApp.Models
{
	public class Constants
	{
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