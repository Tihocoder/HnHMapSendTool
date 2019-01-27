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
		private DoneSessionsWorkType _workType;
		private string _sessionsDirectory;
		private string _moveDirectory;

		public MapSessionsDispatcher(string sessionsDirectory, DoneSessionsWorkType workType, string moveDirectory)
		{
			_workType = workType;
			_sessionsDirectory = sessionsDirectory.TrimEnd('\\');
			_moveDirectory = moveDirectory?.TrimEnd('\\');
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
			string tileFileMask = Properties.Settings.Default.TileFileMask;

			var dirs = Directory.GetDirectories(_sessionsDirectory);
			foreach (var dir in dirs)
			{ 
				DirectoryInfo dirInfo = new DirectoryInfo(dir);

				if (previouslyUploadedSession.Contains(dirInfo.Name))
				{
					continue;
				}

				DateTime sessionDateTime = new DateTime();
				if (!DateTime.TryParseExact(dirInfo.Name, sessionDirMask, CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionDateTime))
				{
					continue;
				}

				if (!dirInfo.GetFiles(tileFileMask).Any())
				{
					if (!dirInfo.GetFiles().Any() && !dirInfo.GetDirectories().Any())
						dirInfo.Delete();
					continue;
				}

				yield return dirInfo.Name;
			}
		}

		public void SessionIsSent(string sessionName)
		{
			switch (_workType)
			{
				case DoneSessionsWorkType.None:
					MarkSessionAsSent(sessionName);
					break;
				case DoneSessionsWorkType.Delete:
					DeleteSession(sessionName);
					break;
				case DoneSessionsWorkType.Move:
					if (String.IsNullOrEmpty(_moveDirectory))
						MarkSessionAsSent(sessionName);
					else
						MoveSession(sessionName, _moveDirectory);
					break;
			}
			
		}

		private void MarkSessionAsSent(string sessionName)
		{
			string previouslyUploadedSessionFile = $"{_sessionsDirectory}\\{Properties.Settings.Default.PreviouslyUploadedSessionFileName}";
			File.AppendAllText(previouslyUploadedSessionFile, sessionName + Environment.NewLine);
		}

		private void DeleteSession(string sessionName)
		{
			DirectoryInfo dirInfo = new DirectoryInfo($"{_sessionsDirectory}\\{sessionName}");
			var files = dirInfo.GetFiles($"ids.txt|{Properties.Settings.Default.TileFileMask}");
			foreach (var file in files)
			{
				file.Delete();
			}
			if (!dirInfo.GetFiles().Any() && !dirInfo.GetDirectories().Any())
				dirInfo.Delete();
		}

		private void MoveSession(string sessionName, string destDirectory)
		{
			DirectoryInfo dirInfo = new DirectoryInfo($"{_sessionsDirectory}\\{sessionName}");
			dirInfo.MoveTo($"{destDirectory}\\{sessionName}");
		}
	}
}
