using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core
{
	internal class MapSessionsDispatcher
	{
		private string _sessionsDirectory;

		public MapSessionsDispatcher(string sessionsDirectory)
		{
			_sessionsDirectory = sessionsDirectory.TrimEnd('\\');
		}

		public IEnumerable<string> GetNewSessions()
		{
			if (!Directory.Exists(_sessionsDirectory))
				yield break;

			HashSet<string> previouslyUploadedSession;
			string previouslyUploadedSessionFile = $"{_sessionsDirectory}\\{Properties.Settings.Default.PreviouslyUploadedSessionFileName}";
			if (File.Exists(previouslyUploadedSessionFile))
				previouslyUploadedSession = new HashSet<string>(File.ReadAllLines(previouslyUploadedSessionFile));
			else
				previouslyUploadedSession = new HashSet<string>();

			string sessionDirMask = Properties.Settings.Default.SessionDirMask;
			string tileFileExtension = Properties.Settings.Default.TileFileExtension;

			var dirs = Directory.GetDirectories(_sessionsDirectory);
			foreach (var dir in dirs)
			{ 
				DirectoryInfo dirInfo = new DirectoryInfo(dir);

				if (previouslyUploadedSession.Contains(dirInfo.Name))
					continue;

				DateTime sessionDateTime = new DateTime();
				if (!DateTime.TryParseExact(dirInfo.Name, sessionDirMask, CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionDateTime))
					continue;

				if (!dirInfo.GetFiles($"*.{tileFileExtension}").Any())
					continue;

				yield return dirInfo.Name;
			}
		}

		public void MarkSessionsAsSent(string sessionName)
		{
			string previouslyUploadedSessionFile = $"{_sessionsDirectory}\\{Properties.Settings.Default.PreviouslyUploadedSessionFileName}";
			File.AppendAllText(previouslyUploadedSessionFile, sessionName + Environment.NewLine);
		}
	}
}
