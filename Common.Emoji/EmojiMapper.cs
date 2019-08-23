using System;
using System.Collections.Generic;

namespace SM.Common.Emoji
{
	public class EmojiMapper
	{
		private Dictionary<string, string> _map;

		public EmojiMapper()
		{
			_map = new Dictionary<string, string>();

			_map.Add("necktie", "");
		}
	}
}
