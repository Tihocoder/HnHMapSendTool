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
			DownloadGlobalСoordinatesCommand = new RelayCommand(DownloadGlobalСoordinates);
		}

		#region propertes

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
		public string UploadUrl
		{
			get
			{
				return Properties.Settings.Default.UploadUrl;
			}
			set
			{
				Properties.Settings.Default.UploadUrl = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(UploadUrl));
			}
		}

		/// <summary>
		/// Логин для принимающего сервера, если надо.
		/// </summary>
		public string UploadUrlLogin
		{
			get
			{
				return Properties.Settings.Default.UploadUrlLogin;
			}
			set
			{
				Properties.Settings.Default.UploadUrlLogin = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(UploadUrlLogin));
			}
		}

		/// <summary>
		/// Пароль для принимающего сервера, если надо.
		/// </summary>
		public string UploadUrlPassword
		{
			get
			{
				return Properties.Settings.Default.UploadUrlPassword;
			}
			set
			{
				Properties.Settings.Default.UploadUrlPassword = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(UploadUrlPassword));
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
		/// Путь для загрузки grid_ids.txt с глобальными координатами
		/// </summary>
		public string DownloadUrl
		{
			get
			{
				return Properties.Settings.Default.DownloadUrl;
			}
			set
			{
				Properties.Settings.Default.DownloadUrl = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(DownloadUrl));
			}
		}

		/// <summary>
		/// Логин для отдающего сервера, если надо.
		/// </summary>
		public string DownloadUrlLogin
		{
			get
			{
				return Properties.Settings.Default.DownloadUrlLogin;
			}
			set
			{
				Properties.Settings.Default.DownloadUrlLogin = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(DownloadUrlLogin));
			}
		}

		/// <summary>
		/// Пароль для отдающего сервера, если надо.
		/// </summary>
		public string DownloadUrlPassword
		{
			get
			{
				return Properties.Settings.Default.DownloadUrlPassword;
			}
			set
			{
				Properties.Settings.Default.DownloadUrlPassword = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(DownloadUrlPassword));
			}
		}

		/// <summary>
		/// Путь, куда класть глобальные координаты D:\Games\HnH\Amber
		/// </summary>
		public string GlobalСoordinatesDirectory
		{
			get
			{
				return Properties.Settings.Default.GlobalСoordinatesDirectory;
			}
			set
			{
				Properties.Settings.Default.GlobalСoordinatesDirectory = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(GlobalСoordinatesDirectory));
			}
		}

		/// <summary>
		/// Обновлять глобальные координаты автоматически, после отправки сессий
		/// </summary>
		public bool IsAutoDownloadGlobalСoordinates
		{
			get
			{
				return Properties.Settings.Default.IsAutoDownloadGlobalСoordinates;
			}
			set
			{
				Properties.Settings.Default.IsAutoDownloadGlobalСoordinates = value;
				Properties.Settings.Default.Save();
				OnPropertyChanged(nameof(IsAutoDownloadGlobalСoordinates));
			}
		}

		#endregion

		/// <summary>
		/// Комманда отправки всех сессий
		/// </summary>
		public ICommand SendAllNewSessionsCommand { get; }

		/// <summary>
		/// Комманда отправки всех сессий
		/// </summary>
		public ICommand DownloadGlobalСoordinatesCommand { get; }

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

			if (String.IsNullOrEmpty(UploadUrl))
			{
				//FIXME: сообщение об ошибках переделать, что бы не исползовать класс Exception, все тексты на уровень интерфейса!
				_errorLoggerCallback?.Invoke(new Exception("Не задан Url, куда отправлять"));
				return;
			}

			MapSessionsDispatcher mapSessionsDispatcher = new MapSessionsDispatcher(SessionsDirectory, WorkType, MoveDirectory);
			var sessions = mapSessionsDispatcher.GetNewSessions().ToList();

			if (sessions != null && sessions.Any())
			{
				//ISender sender = new FileSender(SessionsDirectory);
				ISender sender = new RESTPostSender(UploadUrl, UploadUrlLogin, UploadUrlPassword, SenderName);
				foreach (var session in sessions)
				{
					try
					{
						string responce = sender.Send(PackageCreator.CreateZipPackage(session.FolderPatch), session.Name);
						mapSessionsDispatcher.SessionIsSent(session);
						//FIXME: сообщение об ошибках переделать, что бы не исползовать класс Exception, все тексты на уровень интерфейса!
						_sessionsSentCallback?.Invoke($"Сессия {session.Name} (Фрагменты: {session.FilesCount}): {responce}"); 
					}
					catch (Exception ex)
					{
						_errorLoggerCallback?.Invoke(new Exception($"Сессия {session.Name}: {ex.Message}", ex));
					}
				}
			}
			else
			{
				//TODO: Более вменяемое сообщение об отсутствии файлов
				_errorLoggerCallback?.Invoke(new Exception("Новые файлы сессий не обнаружены"));
			}

			if (IsAutoDownloadGlobalСoordinates)
				DownloadGlobalСoordinates();
		}

		public void DownloadGlobalСoordinates()
		{
			//TODO: Асинхронность, при том, что в .net3.5 нет Task'ов
			if (String.IsNullOrEmpty(GlobalСoordinatesDirectory))
			{
				//FIXME: сообщение об ошибках переделать, что бы не исползовать класс Exception, все тексты на уровень интерфейса!
				_errorLoggerCallback?.Invoke(new Exception("Не задан путь к месту сохранения глобальных координат"));
				return;
			}

			if (String.IsNullOrEmpty(DownloadUrl))
			{
				//FIXME: сообщение об ошибках переделать, что бы не исползовать класс Exception, все тексты на уровень интерфейса!
				_errorLoggerCallback?.Invoke(new Exception("Не задан Url, откуда брать глобальные координаты"));
				return;
			}

			try
			{
				var file = FileDownloader.Download(DownloadUrl, DownloadUrlLogin, DownloadUrlPassword);
				Helper.SaveAsFile(file, $"{GlobalСoordinatesDirectory.TrimEnd('\\')}\\{Properties.Settings.Default.GlobalСoordinatesFileName}", System.IO.FileMode.Create);
				_sessionsSentCallback?.Invoke($"Глобальные координаты обновлены");
			}
			catch (Exception ex)
			{
				_errorLoggerCallback?.Invoke(new Exception($"Ошибка загрузки глобальных координат: {ex.Message}", ex));
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
