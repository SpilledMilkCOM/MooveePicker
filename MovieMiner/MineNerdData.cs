using Newtonsoft.Json;

namespace MovieMiner
{
	public class MineNerdData
	{
		[JsonProperty(PropertyName = "year")]
		public int Year { get; set; }

		[JsonProperty(PropertyName = "season")]
		public string Season { get; set; }

		[JsonProperty(PropertyName = "week")]
		public int Week { get; set; }

		[JsonProperty(PropertyName = "movies")]
		public MineNerdMovie[] Movies { get; set; }
	}
}
