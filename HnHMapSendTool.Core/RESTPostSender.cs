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
		private readonly Encoding _encoding;

		public RESTPostSender(string url, string user, string password, string sender)
		{
			_encoding = Encoding.UTF8;
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
			package.Position = 0;
			byte[] data = new byte[package.Length];
			package.Read(data, 0, data.Length);
			return SendFile(_url, data, $"{packageName}.zip");
		}

		private string SendFile(string url, byte[] data, string filename)
		{
			string boundary = String.Format("----------{0:N}", Guid.NewGuid());

			byte[] body;
			using (MemoryStream bodyStream = new System.IO.MemoryStream())
			{
				string header = $"--{boundary}\r\nContent-Disposition: form-data; name=\"{filename}\"; filename=\"{filename}\";\r\nContent-Type: application/zip\r\n\r\n";
				byte[] headerBytes = _encoding.GetBytes(header);
				bodyStream.Write(headerBytes, 0, headerBytes.Length);
				//Helper.CopyStreamToStream(data, bodyStream);
				bodyStream.Write(data, 0, data.Length);
				string footer = $"\r\n--{boundary}--\r\n";
				byte[] footerBytes = _encoding.GetBytes(footer);
				bodyStream.Write(footerBytes, 0, footerBytes.Length);

				body = bodyStream.ToArray();
			}

			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
			request.Method = "POST";
			request.ContentType = $"multipart/form-data; boundary={boundary}";
			request.UserAgent = "HnHMapSendTool";
			request.CookieContainer = new CookieContainer();
			request.ContentLength = body.Length;
			request.Credentials = _credentials;
			request.KeepAlive = true;
			request.Timeout = 1000*20;

			using (Stream requestStream = request.GetRequestStream())
			{
				requestStream.Write(body, 0, body.Length);
				requestStream.Close();
			}

			WebResponse response = request.GetResponse();

			using (Stream responseStream = response.GetResponseStream())
			{
				StreamReader streamReader = new StreamReader(responseStream);
				string responseData = streamReader.ReadToEnd();
				return responseData;
			}
		}
	}
}
