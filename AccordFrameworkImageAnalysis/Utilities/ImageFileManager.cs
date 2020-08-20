using ReliaCoat.Common.UI.FileManagement;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace AccordFrameworkImageAnalysis.Utilities
{
	public class ImageFileManager : DataFileManager<Bitmap>
	{
		public ImageFileManager()
		{
			_openFileFilterDictionary = new Dictionary<string, IList<string>>
			{
				{ "Image File", new[] { "tif", "jpg" } }
			};
		}

		protected override Task<Bitmap> readFileOverrideAsync(string readFilePath)
		{
			return Task.FromResult(Image.FromFile(readFilePath) as Bitmap);
		}

		protected override Task<bool> writeFileOverrideAsync(Bitmap modelToWrite, string saveFilePath)
		{
			throw new NotImplementedException();
		}
	}
}
