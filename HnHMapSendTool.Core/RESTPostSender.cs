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
		private ICredentials _credentials;
		private string _sender;

		public RESTPostSender(string url, string user, string password, string sender)
		{
			_url = url;
			_sender = sender;
			if (String.IsNullOrEmpty(user))
			{
				_credentials = CredentialCache.DefaultCredentials;
			}
			else
			{
				_credentials = new System.Net.NetworkCredential(user, password);
			}
		}

		public string Send(Stream package, string packageName)
		{
			using (MemoryStream output = new MemoryStream())
			{
				Helper.CopyStreamToStream(package, output);
				return SendFile(_url, output, packageName);
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

		public string SendFile(string url, MemoryStream data, string filename)
		{
			WebResponse response = null;
			var callback = GetApproveAllCertificateCallback();
			try
			{
				System.Net.ServicePointManager.ServerCertificateValidationCallback += callback;
				string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
				byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("/r/n--" + boundary + "/r/n");

				HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
				wr.ContentType = "multipart/form-data; boundary=" + boundary;
				wr.Method = "POST";
				wr.KeepAlive = true;
				wr.Credentials = _credentials;
				using (Stream requestStream = wr.GetRequestStream())
				{
					requestStream.Write(boundarybytes, 0, boundarybytes.Length);
					byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(filename);
					requestStream.Write(formitembytes, 0, formitembytes.Length);
					requestStream.Write(boundarybytes, 0, boundarybytes.Length);
					byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes($"Content-Disposition: form-data; usertag=\"{_sender}\"; name=\"file\"; filename=\"{filename}\"/r/nContent-Type:zip/r/n/r/n");
					requestStream.Write(headerbytes, 0, headerbytes.Length);

					byte[] file = data.ToArray();
					requestStream.Write(file, 0, file.Length);

					byte[] trailer = System.Text.Encoding.ASCII.GetBytes("/r/n--" + boundary + "--/r/n");
					requestStream.Write(trailer, 0, trailer.Length);
					requestStream.Close();
				}
				response = wr.GetResponse();
				using (Stream responseStream = response.GetResponseStream())
				{
					StreamReader streamReader = new StreamReader(responseStream);
					string responseData = streamReader.ReadToEnd();
					return responseData;
				}
			}
			finally
			{
				System.Net.ServicePointManager.ServerCertificateValidationCallback -= callback;
				if (response != null)
					response.Close();
			}
		}
	}
}
