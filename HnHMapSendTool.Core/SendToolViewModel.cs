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

		/// <summary>
		/// Управляющий отправкой компонент.
		/// </summary>
		/// <param name="errorLoggerCallback">Функция для обработки ошибок, возникших в процессе отправки.</param>
		/// <param name="sessionsSentCallback">Функция для обработки результата отправки.</param>
		public SendToolViewModel(Action<Exception> errorLoggerCallback, Action<string> sessionsSentCallback)
		{
			_errorLoggerCallback = errorLoggerCallback;
			_sessionsSentCallback = sessionsSentCallback;
			SendAllNewSessionsCommand = new RelayCommand(SendAllNewSessions);
		}

		/// <summary>
		/// Путь, по которому находятся сессии, например D:\Games\HnH\Amber\map
		/// </summary>
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

		/// <summary>
		/// Сервер, принимающий запакованные сессии.
		/// </summary>
		public string Url
		{
			get
			{
				return Properties.Settings.Default.Url;
			}
			set
			{
				Properties.Settings.Default.Url = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(Url));
			}
		}

		/// <summary>
		/// Логин для принимающего сервера, если надо.
		/// </summary>
		public string UrlLogin
		{
			get
			{
				return Properties.Settings.Default.UrlLogin;
			}
			set
			{
				Properties.Settings.Default.UrlLogin = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(UrlLogin));
			}
		}

		/// <summary>
		/// Пароль для принимающего сервера, если надо.
		/// </summary>
		public string UrlPassword
		{
			get
			{
				return Properties.Settings.Default.UrlPassword;
			}
			set
			{
				Properties.Settings.Default.UrlPassword = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(UrlPassword));
			}
		}

		/// <summary>
		/// Режим, определяющий, что происходит с обработанными сессиями.
		/// </summary>
		public DoneSessionsWorkType WorkType
		{
			get
			{
				return (DoneSessionsWorkType)Properties.Settings.Default.DoneSessionsWorkType;
			}
			set
			{
				Properties.Settings.Default.DoneSessionsWorkType = (int)value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(WorkType));
			}
		}

		/// <summary>
		/// Папка, куда складывать обработанные сессии, если выбран соответствующий режим.
		/// </summary>
		public string MoveDirectory
		{
			get
			{
				return Properties.Settings.Default.MoveDirectory;
			}
			set
			{
				Properties.Settings.Default.MoveDirectory = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(MoveDirectory));
			}
		}

		/// <summary>
		/// Имя/тэг отправителя
		/// </summary>
		public string SenderName
		{
			get
			{
				return Properties.Settings.Default.SenderName;
			}
			set
			{
				Properties.Settings.Default.SenderName = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(SenderName));
			}
		}

		/// <summary>
		/// Комманда отправки всех сессий
		/// </summary>
		public ICommand SendAllNewSessionsCommand { get; }

		/// <summary>
		/// Отправить все сессии
		/// </summary>
		public void SendAllNewSessions()
		{
			//TODO: Асинхронность, при том, что в .net3.5 нет Task'ов
			if (String.IsNullOrEmpty(SessionsDirectory))
			{
				//FIXME: сообщение об ошибках переделать, что бы не исползовать класс Exception, все тексты на уровень интерфейса!
				_errorLoggerCallback?.Invoke(new Exception("Не задан путь к сессиям карты"));
				return;
			}

			if (String.IsNullOrEmpty(Url))
			{
				//FIXME: сообщение об ошибках переделать, что бы не исползовать класс Exception, все тексты на уровень интерфейса!
				_errorLoggerCallback?.Invoke(new Exception("Не задан Url, куда отправлять"));
				return;
			}

			MapSessionsDispatcher mapSessionsDispatcher = new MapSessionsDispatcher(SessionsDirectory, WorkType, MoveDirectory);
			var sessions = mapSessionsDispatcher.GetNewSessions().ToList();

			if (sessions != null && sessions.Any())
			{
				PackageCreator packageCreator = new PackageCreator();
				//ISender sender = new FileSender(SessionsDirectory);
				ISender sender = new RESTPostSender(Url, UrlLogin, UrlPassword, SenderName);
				foreach (string session in sessions)
				{
					try
					{
						string responce = sender.Send(packageCreator.CreateZipPackage($"{SessionsDirectory.TrimEnd('\\')}\\{session}"), session);
						mapSessionsDispatcher.SessionIsSent(session);
						_sessionsSentCallback?.Invoke($"Сессия {session}: {responce}");
					}
					catch (Exception ex)
					{
						_errorLoggerCallback?.Invoke(new Exception($"Сессия {session}: {ex.Message}", ex));
					}
				}
			}
			else
			{
				//TODO: Более вменяемое сообщение об отсутствии файлов
				_errorLoggerCallback?.Invoke(new Exception("Новые файлы сессий не обнаружены"));
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
