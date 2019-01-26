using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HnHMapSendTool.Core
{
	internal class RESTPostSender : ISender
	{
		private string _url;

		public RESTPostSender(string url, string user, string password)
		{
			_url = url;
		}

		public void Send(Stream package, string packageName)
		{
			using (MemoryStream output = new MemoryStream())
			{
				Helper.CopyStreamToStream(package, output);
				SendPostMethod(_url, output);
			}
		}

		private string SendPostMethod(string url, MemoryStream data)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "POST";
			request.Credentials = CredentialCache.DefaultCredentials;

			var bytes = data.ToArray();

			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = bytes.Length;
			request.UserAgent = Properties.Settings.Default.RESTPostUserAgent;
			request.Timeout = Properties.Settings.Default.RESTPostTimeout;
			request.KeepAlive = true;
			//request.Headers.Add(HttpRequestHeader.)

			var callback = GetApproveAllCertificateCallback();
			try
			{
				System.Net.ServicePointManager.ServerCertificateValidationCallback += callback;

				using (var newStream = request.GetRequestStream())
				{
					newStream.Write(bytes, 0, bytes.Length);
					newStream.Close();
				}
				string output;
				using (WebResponse resp = request.GetResponse())
				{
					using (Stream dataStream = resp.GetResponseStream())
					{
						using (StreamReader rdr = new StreamReader(dataStream))
						{
							output = rdr.ReadToEnd();
							rdr.Close();
						}
						dataStream.Close();
					}
					resp.Close();
				}
				return output;
			}
			finally
			{
				System.Net.ServicePointManager.ServerCertificateValidationCallback -= callback;
			}
		}

		//TODOL Проверка сертификата для https (пока что все подходят)
		private System.Net.Security.RemoteCertificateValidationCallback GetApproveAllCertificateCallback()
		{
			return delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
											System.Security.Cryptography.X509Certificates.X509Chain chain,
											System.Net.Security.SslPolicyErrors sslPolicyErrors)
			{
				return true;
			};
		}
	}
}
