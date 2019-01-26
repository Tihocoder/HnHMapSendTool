using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HnHMapSendTool.Core
{
	public class SendToolViewModel : INotifyPropertyChanged
	{
		private Action<Exception> _errorLoggerCallback;
		private Action<string> _sessionsSentCallback;

		public SendToolViewModel(Action<Exception> errorLoggerCallback, Action<string> sessionsSentCallback)
		{
			_errorLoggerCallback = errorLoggerCallback;
			_sessionsSentCallback = sessionsSentCallback;
			SendAllNewSessionsCommand = new RelayCommand(() => SendAllNewSessions(), () => { return !String.IsNullOrEmpty(SessionsDirectory); });
		}

		public string SessionsDirectory
		{
			get
			{
				return Properties.Settings.Default.SessionsDirectory;
			}
			set
			{
				Properties.Settings.Default.SessionsDirectory = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(SessionsDirectory));
			}
		}

		public ICommand SendAllNewSessionsCommand { get; }

		public void SendAllNewSessions()
		{
			MapSessionsDispatcher mapSessionsDispatcher = new MapSessionsDispatcher(SessionsDirectory);
			var sessions = mapSessionsDispatcher.GetNewSessions().ToList();

			if (sessions != null && sessions.Any())
			{
				PackageCreator packageCreator = new PackageCreator();
				ISender sender = new FileSender(SessionsDirectory);
				foreach (string session in sessions)
				{
					try
					{
						sender.Send(packageCreator.CreateZipPackage($"{SessionsDirectory.TrimEnd('\\')}\\{session}"), session);
						mapSessionsDispatcher.MarkSessionsAsSent(session);
						_sessionsSentCallback?.Invoke(session);
					}
					catch (Exception ex)
					{
						_errorLoggerCallback?.Invoke(ex);
					}
				}
			}
			else
			{
				//TODO: Более вменяемое сообщение об отсутствии файлов
				_errorLoggerCallback?.Invoke(new Exception("No new files found"));
			}
		}

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propName)
		{
			var handler = PropertyChanged;
			handler?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

		#endregion
	}
}
