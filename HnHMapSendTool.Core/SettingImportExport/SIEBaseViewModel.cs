using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace HnHMapSendTool.Core.SettingImportExport
{
	public abstract class SIEBaseViewModel : NotifyPropertyViewModel
	{
		protected const char SEPARATOR = ':';

		protected string _settingsText;

		public IEnumerable<SettingItem> Settings { get; protected set; }
		public String SettingsText
		{
			get { return _settingsText; }
			set
			{
				_settingsText = value;
				OnSettingsTextChanged();
				OnPropertyChanged(nameof(SettingsText));
			}
		}
		public bool IsSettingsTextReadOnly { get; protected set; }
		public ICommand DoCommand { get; protected set; }
		public Action OnWorkDone { get; set; }
		public abstract string ActionTitle { get; }

		protected virtual void OnSettingsTextChanged() { }

		private static Dictionary<string, string> _names = new Dictionary<string, string>
		{
			{ nameof(SendToolViewModel.SenderName), "Имя отправителя" }
			, { nameof(SendToolViewModel.UploadUrl), "Принимающий URL" }
			, { nameof(SendToolViewModel.UploadUrlLogin), "Логин для тайлов" }
			, { nameof(SendToolViewModel.UploadUrlPassword), "Пароль для тайлов" }
			, { nameof(SendToolViewModel.DownloadUrl), "URL для координат" }
			, { nameof(SendToolViewModel.DownloadUrlLogin), "Логин для координат" }
			, { nameof(SendToolViewModel.DownloadUrlPassword), "Пароль для координат" }
			//, { nameof(SendToolViewModel.IsAutoDownloadGlobalСoordinates), "" }
			//, { nameof(SendToolViewModel.UploadUrlLogin), "" }
			//, { nameof(SendToolViewModel.UploadUrlLogin), "" }
		};

		public static string GetItemName(string title)
		{
			return _names.ContainsKey(title) ? _names[title] : title;
		}
	}
}
