using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HnHMapSendTool.Core
{
	internal class FileDownloader
	{
		public static MemoryStream Download(string url, string user, string password)
		{
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			request.Method = "GET";
			request.UserAgent = "HnHMapSendTool";
			request.CookieContainer = new CookieContainer();
			request.KeepAlive = true;
			request.Timeout = Properties.Settings.Default.RequestsTimeouts;

			if (String.IsNullOrEmpty(user))
			{
				request.Credentials = CredentialCache.DefaultCredentials;
			}
			else
			{
				request.Credentials = new System.Net.NetworkCredential(user, password);
			}

			WebResponse response = request.GetResponse();

			using (Stream responseStream = response.GetResponseStream())
			{
				StreamReader streamReader = new StreamReader(responseStream);
				return new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd()));
				/*MemoryStream output = new MemoryStream();

				byte[] buffer = new byte[response.ContentLength];

				responseStream.Read(buffer, 0, buffer.Length);
				output.Write(buffer, 0, buffer.Length);

				return output;*/
			}
		}
	}
}
