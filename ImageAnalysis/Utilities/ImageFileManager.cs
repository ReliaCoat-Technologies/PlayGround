using System.Collections.Generic;
using ReliaCoat.Common.UI.FileManagement;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageAnalysis.Utilities
{
	public class ImageFileManager : DataFileManager<Bitmap>
	{
		public ImageFileManager()
		{
			_openFileFilterDictionary = new Dictionary<string, IList<string>>
			{
				{ "Image File", new List<string> {"tif", "jpg"} }
			};
		}

		protected override Task<Bitmap> readFileOverrideAsync(string readFilePath)
		{
			var image = Image.FromFile(readFilePath);

			return Task.FromResult(image as Bitmap);
		}

		protected override Task<bool> writeFileOverrideAsync(Bitmap modelToWrite, string saveFilePath)
		{
			throw new System.NotImplementedException();
		}
	}
}