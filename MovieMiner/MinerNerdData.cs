using Newtonsoft.Json;

namespace MovieMiner
{
	public class MinerNerdMovie
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

	public class MinerNerdData
	{
		[JsonProperty(PropertyName = "year")]
		public int Year { get; set; }

		[JsonProperty(PropertyName = "season")]
		public string Season { get; set; }

		[JsonProperty(PropertyName = "week")]
		public int Week { get; set; }

		[JsonProperty(PropertyName = "movies")]
		public MinerNerdMovie[] Movies { get; set; }
	}
}
