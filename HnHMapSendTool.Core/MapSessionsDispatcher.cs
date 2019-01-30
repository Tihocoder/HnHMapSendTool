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
		private const char FILES_COUNT_SEPARATOR = '|';

		private DoneSessionsWorkType _workType;
		private string _sessionsDirectory;
		private string _moveDirectory;

		public MapSessionsDispatcher(string sessionsDirectory, DoneSessionsWorkType workType, string moveDirectory)
		{
			_workType = workType;
			_sessionsDirectory = sessionsDirectory.TrimEnd('\\');
			_moveDirectory = moveDirectory?.TrimEnd('\\');
		}

		public IEnumerable<HnHMapSession> GetNewSessions()
		{
			if (!Directory.Exists(_sessionsDirectory))
				yield break;

			Dictionary<string, int> previouslyUploadedSession;
			string previouslyUploadedSessionFile = $"{_sessionsDirectory}\\{Properties.Settings.Default.PreviouslyUploadedSessionFileName}";
			if (File.Exists(previouslyUploadedSessionFile))
				previouslyUploadedSession = File.ReadAllLines(previouslyUploadedSessionFile)
					.Where(row => row.Contains(FILES_COUNT_SEPARATOR)) //TODO: Валидация формата с проверкой, что за FILES_COUNT_SEPARATOR стоит число
					.Select(el => new { datetime = el.Split(FILES_COUNT_SEPARATOR)[0], filescount = Convert.ToInt32(el.Split(FILES_COUNT_SEPARATOR)[1]) })
					.GroupBy(folderssinfo => folderssinfo.datetime)
					.ToDictionary(key => key.Key, value => value.Select(groupeditems => groupeditems.filescount).Max());
			else
				previouslyUploadedSession = new Dictionary<string, int>();

			string sessionDirMask = Properties.Settings.Default.SessionDirMask;
			string tileFileMask = Properties.Settings.Default.TileFileMask;

			var dirs = Directory.GetDirectories(_sessionsDirectory);
			foreach (var dir in dirs)
			{ 
				DirectoryInfo dirInfo = new DirectoryInfo(dir);
				int filecount = dirInfo.GetFiles(tileFileMask).Count();

				if (previouslyUploadedSession.ContainsKey(dirInfo.Name) && previouslyUploadedSession[dirInfo.Name] == filecount)
				{
					continue;
				}

				DateTime sessionDateTime = new DateTime();
				if (!DateTime.TryParseExact(dirInfo.Name, sessionDirMask, CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionDateTime))
				{
					continue;
				}

				if (filecount == 0)
				{
					SessionIsSent(new HnHMapSession() { Name = dirInfo.Name, FolderPatch = dir, FilesCount = 0 });
					continue;
				}

				yield return new HnHMapSession() { Name = dirInfo.Name, FolderPatch = dir, FilesCount = filecount };
			}
		}

		public void SessionIsSent(HnHMapSession session)
		{
			switch (_workType)
			{
				case DoneSessionsWorkType.None:
					MarkSessionAsSent(session);
					break;
				case DoneSessionsWorkType.Delete:
					DeleteSession(session);
					break;
				case DoneSessionsWorkType.Move:
					if (String.IsNullOrEmpty(_moveDirectory))
						MarkSessionAsSent(session);
					else
						MoveSession(session, _moveDirectory);
					break;
			}
			
		}

		private void MarkSessionAsSent(HnHMapSession session)
		{
			string previouslyUploadedSessionFile = $"{_sessionsDirectory}\\{Properties.Settings.Default.PreviouslyUploadedSessionFileName}";
			File.AppendAllText(previouslyUploadedSessionFile, $"{session.Name}{FILES_COUNT_SEPARATOR}{session.FilesCount}{Environment.NewLine}");
		}

		private void DeleteSession(HnHMapSession session)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(session.FolderPatch);
			var files = dirInfo.GetFiles($"ids.txt|{Properties.Settings.Default.TileFileMask}");
			foreach (var file in files)
			{
				file.Delete();
			}
			if (!dirInfo.GetFiles().Any() && !dirInfo.GetDirectories().Any())
				dirInfo.Delete();
		}

		private void MoveSession(HnHMapSession session, string destDirectory)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(session.FolderPatch);
			dirInfo.MoveTo($"{destDirectory}\\{session.Name}");
		}
	}
}
