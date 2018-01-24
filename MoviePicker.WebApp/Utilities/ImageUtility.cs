using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;

namespace MoviePicker.WebApp.Utilities
{
	public class ImageUtility
	{
		/// <summary>
		/// Combine the movie images into a single image to be shared.
		/// </summary>
		/// <returns>The file name to be served up.</returns>
		private const string DEFAULT_IMAGE_DIR = "images";

		/// <summary>
		/// Every so often clean up the images.
		/// </summary>
		public void CleanupImages()
		{

		}

		public string CombineImages(string webRootPath, List<string> fileNames)
		{
			// There is a cool site that puts images together https://www.fotor.com/create/collage/

			var imagePath = $"{webRootPath}{Path.DirectorySeparatorChar}{DEFAULT_IMAGE_DIR}";
			string resultFileName = null;
			int width = 0;
			int height = 0;
			int offset = 0;

			// Determine height (max height) and width (total width) of the images so they can be placed side by side.

			foreach (var fileName in fileNames)
			{
				using (var image = Image.FromFile($"{imagePath}{Path.DirectorySeparatorChar}{fileName}"))
				{
					width += image.Width;

					if (image.Height > height)
					{
						height = image.Height;
					}
				}
			}

			using (var destBitmap = new Bitmap(width, height))
			{
				using (var graphics = Graphics.FromImage(destBitmap))
				{
					graphics.Clear(Color.Black);

					foreach (var fileName in fileNames)
					{
						using (var image = Image.FromFile($"{imagePath}{Path.DirectorySeparatorChar}{fileName}"))
						{
							graphics.DrawImage(image, offset, 0, image.Width, image.Height);

							offset += image.Width;
						}
					}
				}

				resultFileName = $"{imagePath}{Path.DirectorySeparatorChar}output.jpg";

				destBitmap.Save(resultFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			}

			return resultFileName;
		}
	}
}