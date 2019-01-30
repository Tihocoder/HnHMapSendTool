using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HnHMapSendTool.Core
{
	internal class PackageCreator : IDisposable
	{
		private object external;
		private PackageCreator() { }

		public static Stream CreateZipPackage(string directory)
		{
			DirectoryInfo sourceDir = new DirectoryInfo(directory);
			List<FileInfo> files = sourceDir.GetFiles(Properties.Settings.Default.IdsFileName).ToList();
			files.AddRange(sourceDir.GetFiles(Properties.Settings.Default.TileFileMask));

			Stream packageStream = new MemoryStream();

			using (var package = CreateZip(packageStream))
			{
				foreach (var file in files)
				{
					using (FileStream dataStream = file.Open(FileMode.Open, FileAccess.Read))
					{
						using (var zipStream = package.AddFile($"{sourceDir.Name}/{file.Name}"))
						{
							Helper.CopyStreamToStream(dataStream, zipStream);
							zipStream.Close();
						}
						dataStream.Close();
					}
				}
			}

			return packageStream;
		}

		private static PackageCreator CreateZip(Stream stream)
		{
			var type = typeof(System.IO.Packaging.Package).Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
			var meth = type.GetMethod("OpenOnStream", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			return new PackageCreator { external = meth.Invoke(null, new object[] { stream, FileMode.OpenOrCreate, FileAccess.ReadWrite, false }) };
		}

		public Stream AddFile(string path)
		{
			var type = external.GetType();
			var meth = type.GetMethod("AddFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			var comp = type.Assembly.GetType("MS.Internal.IO.Zip.CompressionMethodEnum").GetField("Deflated").GetValue(null);
			var opti = type.Assembly.GetType("MS.Internal.IO.Zip.DeflateOptionEnum").GetField("Normal").GetValue(null);

			var fileInfo = meth.Invoke(external, new object[] { path, comp, opti });
			var fileInfoGetStream = fileInfo.GetType().GetMethod("GetStream", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			return (Stream)fileInfoGetStream.Invoke(fileInfo, new object[] { FileMode.OpenOrCreate, FileAccess.Write });
		}

		public void Dispose()
		{
			((IDisposable)external).Dispose();
		}
	}
}
