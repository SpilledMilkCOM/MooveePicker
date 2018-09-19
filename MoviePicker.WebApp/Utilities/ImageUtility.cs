using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

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
		/// Adjust the file aspect ratio while preserving the width.
		/// </summary>
		/// <param name="localFile">The path to the file</param>
		/// <param name="aspectRatio">The specified aspect ratio</param>
		/// <returns></returns>
		public string AdjustAspectRatio(string localFile, decimal aspectRatio)
		{
			var tempFileName = $"{Path.GetDirectoryName(localFile)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(localFile)}.temp{Path.GetExtension(localFile)}";

			// Delete the temporary destination file if it exists.

			if (File.Exists(tempFileName))
			{
				File.Delete(tempFileName);
			}

			if (File.Exists(localFile))
			{
				// Move actual file to the temporary name.

				File.Move(localFile, tempFileName);

				using (var image = Image.FromFile(tempFileName))
				{
					// Make sure the aspect ratio needs to be adjusted before adjusting it.

					if (image.Width / aspectRatio != image.Height)
					{
						using (var destBitmap = new Bitmap(image.Width, (int)(image.Width / aspectRatio)))
						{
							using (var graphics = Graphics.FromImage(destBitmap))
							{
								graphics.Clear(Color.Black);

								graphics.DrawImage(image, 0, 0, image.Width, image.Height);
							}

							// Save the resized image to the 

							destBitmap.Save(localFile, System.Drawing.Imaging.ImageFormat.Jpeg);
						}
					}
					else
					{
						// No changes needed so move the file back.

						File.Move(tempFileName, localFile);
					}
				}
			}

			// Rename doesn't work because the local file is still in use.

			//File.Delete(localFile);
			//File.Move(tempFileName, localFile);

			return tempFileName;
		}

		/// <summary>
		/// Adjust the size of the image to the specified width and height.
		/// </summary>
		/// <param name="localFile">The path to the file</param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public string AdjustSize(string localFile, decimal width, decimal height)
		{
			var aspectRatio = width / height;
			var tempFileName = $"{Path.GetDirectoryName(localFile)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(localFile)}.temp{Path.GetExtension(localFile)}";

			// Delete the temporary destination file if it exists.

			if (File.Exists(tempFileName))
			{
				File.Delete(tempFileName);
			}

			if (File.Exists(localFile))
			{
				// Move actual file to the temporary name.

				File.Move(localFile, tempFileName);

				using (var image = Image.FromFile(tempFileName))
				{
					// Make sure the aspect ratio needs to be adjusted before adjusting it.

					if (image.Width / aspectRatio != image.Height)
					{
						using (var destBitmap = new Bitmap((int)width, (int)height))
						{
							using (var graphics = Graphics.FromImage(destBitmap))
							{
								graphics.Clear(Color.Black);

								graphics.DrawImage(image, 0, 0, image.Width, image.Height);
							}

							// Save the resized image to the 

							destBitmap.Save(localFile, System.Drawing.Imaging.ImageFormat.Jpeg);
						}
					}
					else
					{
						// No changes needed so move the file back.

						File.Move(tempFileName, localFile);
					}
				}
			}

			// Rename doesn't work because the local file is still in use.

			//File.Delete(localFile);
			//File.Move(tempFileName, localFile);

			return tempFileName;
		}

		/// <summary>
		/// Combine the list of images into a single image
		/// </summary>
		/// <param name="webRootPath"></param>
		/// <param name="fileNames"></param>
		/// <returns>The result file name.</returns>
		public string CombineImages(string webRootPath, List<string> fileNames, string bonusFileName = null, List<string> filmCellNames = null)
		{
			const int COLUMNS = 4;
			const int BONUS_INSET_PIXELS = 10;
			const int BONUS_SCALE = 4;
			const int BORDER_PIXELS = 0;
			const int CELL_HEIGHT_PIXELS = 46;
			const int FIRST_CELL_HEIGHT_PIXELS = 34;

			// There is a cool site that puts images together https://www.fotor.com/create/collage/

			var guid = Guid.NewGuid();
			var outFileName = $"Shared_{guid}.jpg";
			var twitterFileName = $"Twitter_{guid}.jpg";
			var imagePath = $"{webRootPath}{Path.DirectorySeparatorChar}{DEFAULT_IMAGE_DIR}";
			// Just in case only a few cell names are sent in, the original list can be used.
			var savedFilmCellNames = filmCellNames == null ? null : new List<string>(filmCellNames);
			string resultFileName = null;
			int? minWidth = null;
			int? minHeight = null;
			int width = 0;
			int height = 0;
			int offset = 0;
			int oldWidth = 0;
			int oldHeight = 0;

			// The default is 8 (or less) picks

			// Determine width and height of the 1st row.

			foreach (var fileName in fileNames)
			{
				using (var image = Image.FromFile(fileName))
				{
					if (!minWidth.HasValue || minWidth.Value > image.Width)
					{
						minWidth = image.Width;
						minHeight = image.Height;
					}
				}
			}

			// Use a constant width for each image.

			width = (minWidth.Value + BORDER_PIXELS * 2) * COLUMNS;

			// Since the aspect ratio should be the same for each image then the height will be the same as well (using a constant width)

			height = (minHeight.Value + BORDER_PIXELS * 2) * 2;

			using (var destBitmap = new Bitmap(width, height))
			{
				using (var graphics = Graphics.FromImage(destBitmap))
				{
					using (var bonusBorderBrush = new SolidBrush(Color.LightGreen))
					{
						var column = 0;

						// Fill the background with the "border" color
						//graphics.Clear(Color.White);
						graphics.Clear(Color.Black);

						foreach (var fileName in fileNames)
						{
							using (Image image = Image.FromFile(fileName)
								, plusImage = Image.FromFile($"{imagePath}{Path.DirectorySeparatorChar}green-plus-hi.png"))
							{
								if (column % COLUMNS == 0)
								{
									offset = BORDER_PIXELS;
								}

								var yLoc = column < COLUMNS ? BORDER_PIXELS : minHeight.Value + BORDER_PIXELS * 3;

								if (fileName == bonusFileName)
								{
									var rect = new Rectangle(offset - BORDER_PIXELS, yLoc - BORDER_PIXELS, minWidth.Value + 2 * BORDER_PIXELS, minHeight.Value + 2 * BORDER_PIXELS);

									graphics.FillRectangle(bonusBorderBrush, rect);
								}

								graphics.DrawImage(image, offset, yLoc, minWidth.Value, minHeight.Value);

								if (fileName == bonusFileName)
								{
									// The plus image is effectively square so use 1/6 of the width of the poster size.

									graphics.DrawImage(plusImage
											, offset + minWidth.Value * (BONUS_SCALE - 1) / BONUS_SCALE - BONUS_INSET_PIXELS
											, yLoc + minHeight.Value - minWidth.Value / BONUS_SCALE - BONUS_INSET_PIXELS
											, minWidth.Value / BONUS_SCALE
											, minWidth.Value / BONUS_SCALE);
								}

								offset += minWidth.Value + BORDER_PIXELS * 2;
								column++;
							}
						}
					}
				}

				resultFileName = $"{imagePath}{Path.DirectorySeparatorChar}{outFileName}";

				destBitmap.Save(resultFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			}

			// Create the Twitter image with the 2:1 aspect ratio

			oldWidth = width;
			oldHeight = height;

			// Twitter render size for large format.
			width = 600;
			//height = 300;
			height = 314;

			// Scale the image width (down) proportionate to the height.
			oldWidth = (int)(oldWidth * (double)height / oldHeight);
			offset = (width - oldWidth) / 2;

			int logoWidth = offset / 2;
			int logoInset = (offset + logoWidth) / 2;

			using (var destBitmap = new Bitmap(width, height))
			{
				destBitmap.SetResolution(72, 72);

				using (var graphics = Graphics.FromImage(destBitmap))
				{
					var entireViewportClip = new Rectangle(0, 0, width, height);

					graphics.Clear(Color.Black);

					// Draw the previous image into the horizontally padded bitmap.

					using (var image = Image.FromFile(resultFileName))
					{
						// Using the specified widths will scale the image into the smaller Twitter image.

						graphics.DrawImage(image, offset, 0, oldWidth, height);
					}

					// Keep track of the posters in the film cells for BOTH left and right sides.

					int totalCount = 0;

					// Draw the posters down the left.

					for (var count = 0; count < 7; count++, totalCount++)
					{
						if (filmCellNames != null && filmCellNames.Count == 0)
						{
							// Start over if the list is empty.

							filmCellNames = new List<string>(savedFilmCellNames);
						}

						var cellFileName = (filmCellNames == null || filmCellNames.Count == 0) ? fileNames[totalCount % fileNames.Count] : RemoveRandomItem(filmCellNames);

						using (var image = Image.FromFile(cellFileName))
						{
							var yOffset = FIRST_CELL_HEIGHT_PIXELS - CELL_HEIGHT_PIXELS;

							if (count == 1)
							{
								yOffset = FIRST_CELL_HEIGHT_PIXELS;
							}
							else if (count > 1)
							{
								yOffset = (count - 1) * CELL_HEIGHT_PIXELS + FIRST_CELL_HEIGHT_PIXELS;
							}

							// Clip the viewport to the film cell height.

							var clipRect = new Rectangle(0, yOffset, offset, CELL_HEIGHT_PIXELS);

							graphics.SetClip(clipRect);

							// Scale the poster to fit within the offset
							// Adjust Y value so the image is centered within the film cell

							var scaledHeight = (double)image.Height / image.Width * offset;

							graphics.DrawImage(image, 0, yOffset - (int)(scaledHeight - CELL_HEIGHT_PIXELS) / 2, offset, (int)scaledHeight);
						}
					}

					graphics.SetClip(entireViewportClip);

					// Draw branding on the left.

					using (var image = Image.FromFile($"{imagePath}{Path.DirectorySeparatorChar}Moovee Picker Logo Vertical Strip.png"))
					{
						// Scale the branding to fit within the offset
						graphics.DrawImage(image, 0, 0, offset, (int)((double)image.Height / image.Width * offset));
					}

					// Draw the posters down the right.

					for (var count = 0; count < 7; count++, totalCount++)
					{
						if (filmCellNames != null && filmCellNames.Count == 0)
						{
							// Start over if the list is empty.

							filmCellNames = new List<string>(savedFilmCellNames);
						}

						var cellFileName = (filmCellNames == null || filmCellNames.Count == 0) ? fileNames[totalCount % fileNames.Count] : RemoveRandomItem(filmCellNames);

						using (var image = Image.FromFile(cellFileName))
						{
							var yOffset = FIRST_CELL_HEIGHT_PIXELS - CELL_HEIGHT_PIXELS;

							if (count == 1)
							{
								yOffset = FIRST_CELL_HEIGHT_PIXELS;
							}
							else if (count > 1)
							{
								yOffset = (count - 1) * CELL_HEIGHT_PIXELS + FIRST_CELL_HEIGHT_PIXELS;
							}

							// Clip the viewport to the film cell height.

							var clipRect = new Rectangle(width - offset, yOffset, offset, CELL_HEIGHT_PIXELS);

							graphics.SetClip(clipRect);

							// Scale the poster to fit within the offset
							// Adjust Y value so the image is centered within the film cell

							var scaledHeight = (double)image.Height / image.Width * offset;

							graphics.DrawImage(image, width - offset, yOffset - (int)(scaledHeight - CELL_HEIGHT_PIXELS) / 2, offset, (int)((double)image.Height / image.Width * offset));
						}
					}

					graphics.SetClip(entireViewportClip);

					// Draw branding on the right.

					using (var image = Image.FromFile($"{imagePath}{Path.DirectorySeparatorChar}Moovee Picker Logo Vertical Strip Right.png"))
					{
						// Scale the branding to fit within the offset
						graphics.DrawImage(image, width - offset, 0, offset, (int)((double)image.Height / image.Width * offset));
					}
				}

				resultFileName = $"{imagePath}{Path.DirectorySeparatorChar}{twitterFileName}";

				destBitmap.Save(resultFileName, System.Drawing.Imaging.ImageFormat.Jpeg);

				// TODO: Add a black border/bleed for Twitter while still keeping the 2:1 aspect ratio.
			}

			return outFileName;
		}

		/// <summary>
		/// Apply a mask to a base image.
		/// </summary>
		/// <param name="baseImageName">The base image to which the mask is applied.</param>
		/// <param name="maskImageName">The mask image to apply.</param>
		/// <param name="resultImageName">The result file with the base and the applied mask.</param>
		/// <returns></returns>
		public bool MaskImage(string baseImageName, string maskImageName, string resultImageName)
		{
			var baseDirectory = Path.GetDirectoryName(baseImageName);
			int height = 0;
			int width = 0;
			bool result = false;

			using (var image = Image.FromFile(baseImageName))
			{
				height = image.Height;
				width = image.Width;
			}

			using (var destBitmap = new Bitmap(width, height))
			{
				destBitmap.SetResolution(72, 72);

				using (var graphics = Graphics.FromImage(destBitmap))
				{
					graphics.Clear(Color.Black);

					// Draw the base image into the new bitmap of the same dimensions (copy).

					using (var image = Image.FromFile(baseImageName))
					{
						graphics.DrawImage(image, 0, 0, width, height);
					}

					var maskImagePath = Path.Combine(baseDirectory, maskImageName);

					using (var image = Image.FromFile(maskImagePath))
					{
						graphics.DrawImage(image, 0, 0, image.Width, image.Height);
					}
				}

				var resultFileName = Path.Combine(baseDirectory, resultImageName);

				destBitmap.Save(resultFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
			}

			return result;
		}

		private string RemoveRandomItem(List<string> list)
		{
			var index = new Random().Next(list.Count);
			string result = list.ElementAt(index);

			if(result != null)
			{
				list.Remove(result);
			}

			return result;
		}
	}
}