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
			source.Seek(0, SeekOrigin.Begin);
			byte[] buffer = new byte[64 * 1024];
			int read;
			while ((read = source.Read(buffer, 0, buffer.Length)) > 0)
			{
				dest.Write(buffer, 0, read);
			}
		}
	}
}
