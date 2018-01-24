using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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

		/// <summary>
		/// Combine the list of images into a single image
		/// </summary>
		/// <param name="webRootPath"></param>
		/// <param name="fileNames"></param>
		/// <returns>The result file name.</returns>
		public string CombineImages(string webRootPath, List<string> fileNames)
		{
			// There is a cool site that puts images together https://www.fotor.com/create/collage/

			var imagePath = $"{webRootPath}{Path.DirectorySeparatorChar}{DEFAULT_IMAGE_DIR}";
			string resultFileName = null;
			int width = 0;
			int width2 = 0;
			int height = 0;
			int height2 = 0;
			int offset = 0;

			// The default is 8 (or less) picks

			// Determine width and height of the 1st row.

			foreach (var fileName in fileNames.Take(4))
			{
				using (var image = Image.FromFile(fileName))
				{
					width += image.Width;

					if (height < image.Height)
					{
						height = image.Height;
					}
				}
			}

			// Determine width and height of the 1st row.

			foreach (var fileName in fileNames.Skip(4))
			{
				using (var image = Image.FromFile(fileName))
				{
					width2 += image.Width;

					if (height2 < image.Height)
					{
						height2 = image.Height;
					}
				}
			}

			if (width < width2)
			{
				width = width2;
			}

			using (var destBitmap = new Bitmap(width, height + height2))
			{
				using (var graphics = Graphics.FromImage(destBitmap))
				{
					var column = 0;

					graphics.Clear(Color.Black);

					foreach (var fileName in fileNames)
					{
						using (var image = Image.FromFile(fileName))
						{
							if (column % 4 == 0)
							{
								offset = 0;
							}

							graphics.DrawImage(image, offset, column < 4 ? 0 : height, image.Width, image.Height);

							offset += image.Width;
							column++;
						}
					}
				}

				resultFileName = $"{imagePath}{Path.DirectorySeparatorChar}output.jpg";

				destBitmap.Save(resultFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			}

			return "output.jpg";
		}
	}
}