namespace MoviePicker.Common
{
	/// <summary>
	/// This class is used for a "fuzzy" match for movie titles.
	/// </summary>
	public class TitleMatch
	{
		public decimal Match(string title1, string title2)
		{
			bool comparison = false;
			decimal matchRatio = 0;

			comparison = title1.Equals(title2);

			if (comparison)
			{
				matchRatio = 1;      // An exact match.
			}

			if (!comparison)
			{
				// Not an exact match so try starts with (limited contains)
				// (If contains is used the smaller titles like "It" may match where you don't want it to.)

				if (title1.StartsWith(title2))
				{
					matchRatio = (decimal)title2.Length / title1.Length;

					comparison = true;
				}
				else if (title2.StartsWith(title1))
				{
					matchRatio = (decimal)title1.Length / title2.Length;

					comparison = true;
				}
			}

			if (!comparison)
			{
				int matches = 0;

				// Compare the first X characters

				for (int index = 0; index < title1.Length && index < title2.Length; index++)
				{
					if (title1[index] == title2[index])
					{
						matches++;
					}
					else
					{
						// Stop once there are no matches.
						break;
					}
				}

				matchRatio = (decimal)matches / title1.Length;

				var matchRatio2 = (decimal)matches / title2.Length;

				if (matchRatio < matchRatio2)
				{
					matchRatio = matchRatio2;
				}
			}

			//if (!comparison)
			//{
			//	char[] delimiters = " ".ToCharArray();

			//	// Compare words.
			//	var movieWords = MovieName.Split(delimiters);
			//	var testWords = testMovieName.Split(delimiters);
			//	int matches = 0;

			//	for (int index = 0; index < movieWords.Length; index++)
			//	{
			//		for (int i = 0; i < testWords.Length; i++)
			//		{
			//			if(movieWords[index] != null && movieWords[index] == testWords[i])
			//			{
			//				// Only needs to match once.
			//				matches++;
			//				break;
			//			}
			//		}
			//	}

			//	result = movieWords.Length > 0 && matches / movieWords.Length >= 0.666666;
			//}

			return matchRatio;
		}
	}
}