namespace MoviePicker.Cognitive.Parameters
{
	public enum VisualFeature
	{
		Adult,          // detects if the image is pornographic in nature (depicts nudity or a sex act). Sexually suggestive content is also detected.
		Brands,         // detects various brands within an image, including the approximate location. The Brands argument is only available in English.
		Categories,     // categorizes image content according to a taxonomy defined in documentation.
		Color,          // determines the accent color, dominant color, and whether an image is black&white.
		Description,    // describes the image content with a complete sentence in supported languages.
		Faces,          // detects if faces are present. If present, generate coordinates, gender and age.
		ImageType,      // detects if image is clipart or a line drawing.
		Objects,        // detects various objects within an image, including the approximate location. The Objects argument is only available in English.
		Tags            // tags the image with a detailed list of words related to the image content.
	}
}