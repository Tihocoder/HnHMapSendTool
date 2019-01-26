using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core
{
	internal class PackageCreator
	{
		public Stream CreateZipPackage(string directory)
		{
			DirectoryInfo sourceDir = new DirectoryInfo(directory);
			var files = sourceDir.GetFiles();

			Stream packageStream = new MemoryStream();
			using (Package package = ZipPackage.Open(packageStream, FileMode.Create))
			{
				foreach (var file in files)
				{
					PackagePart document = package.CreatePart(new Uri($"/{file.Name}", UriKind.Relative), ""); ///{sourceDir.Name}
					using (FileStream dataStream = file.Open(FileMode.Open, FileAccess.Read))
					{
						using (var zipStream = document.GetStream())
						{
							Helper.CopyStreamToStream(dataStream, zipStream);
							zipStream.Close();
						}
						dataStream.Close();
					}
				}
				package.Close();
			}
			return packageStream;
		}

	}
}
