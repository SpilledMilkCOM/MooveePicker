using Newtonsoft.Json;

namespace MovieMiner
{
	public class MineNerdMovie
	{
		[JsonProperty(PropertyName = "i")]
		public int Index { get; set; }

		[JsonProperty(PropertyName = "filmTitle")]
		public string Title { get; set; }

		[JsonProperty(PropertyName = "b")]
		public int Bux { get; set; }

		[JsonProperty(PropertyName = "origEstBO")]
		public int OriginalEstimatedBoxOffice { get; set; }

		[JsonProperty(PropertyName = "lastWeekBO")]
		public int LastWeekBoxOffice { get; set; }

		[JsonProperty(PropertyName = "lastWeekDrop")]
		public string LastWeekDrop { get; set; }

		[JsonProperty(PropertyName = "currEstBO")]
		public int CurrentEstimatedBoxOffice { get; set; }

		[JsonProperty(PropertyName = "img")]
		public string ImageUrl { get; set; }
	}
}
