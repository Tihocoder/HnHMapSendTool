using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core
{
	internal static class Helper
	{
		/// <summary>
		/// .CopyTo() is missing in .net 3.5
		/// </summary>
		/// <param name="source">Stream for read from start to end</param>
		/// <param name="dest">Stream for append source</param>
		public static void CopyStreamToStream(Stream source, Stream dest)
		{
			byte[] buffer = new byte[source.Length];
			source.Seek(0, SeekOrigin.Begin);
			source.Read(buffer, 0, buffer.Length);
			dest.Write(buffer, 0, buffer.Length);
		}

		public static void SaveAsFile(Stream source, string filepatch, FileMode mode)
		{
			using (FileStream fs = new FileStream(filepatch, mode))
			{
				CopyStreamToStream(source, fs);
			}
		}
	}
}
