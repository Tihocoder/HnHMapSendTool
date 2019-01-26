using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace HnHMapSendTool.WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
		private static string ERROR_FILENAME = System.IO.Directory.GetCurrentDirectory().TrimEnd('\\') + "\\error.log";

		public static void LogError(Exception ex)
		{
			LogToFile(string.Format
				(
					"\t{0}{1}\t-\t{2}\t:\t{3}{0}{4}{0}Assebly Version: {5}",
					Environment.NewLine,
					DateTime.Now,
					ex.GetType(),
					ex.Message,
					GetStackTrace(ex),

					System.Reflection.Assembly.GetEntryAssembly().GetName().Version
				)
			, ERROR_FILENAME);

			if (ex.InnerException != null)
			{
				LogToFile(Environment.NewLine + "InnerException : ", ERROR_FILENAME);
				LogError(ex.InnerException);
			}

		}

		private static void LogToFile(string msg, string file)
		{
			try
			{
				msg = string.Format("{0:G}: {1}\r\n", DateTime.Now, msg);
				File.AppendAllText(file, msg + Environment.NewLine);
			}
			catch (Exception)
			{
				//ошибка в логгере ошибок - это плохо...
			}
		}

		private static string GetStackTrace(Exception ex)
		{
			if (ex.StackTrace == null)
				return "";
			string[] trace = ex.StackTrace.Split('\n');
			StringBuilder sb = new StringBuilder();
			foreach (string item in trace)
			{
				if (item.IndexOf("HnHMapSendTool") > 0)
					sb.AppendLine(item);
			}
			return sb.ToString();
		}
	}

}
