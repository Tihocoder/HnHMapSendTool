using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core.SettingImportExport
{
	public class SettingExportViewModel : SIEBaseViewModel
	{
		public SettingExportViewModel(IEnumerable<SettingItem> settings)
		{
			IsSettingsTextReadOnly = true;

			DoCommand = new RelayCommand(() => { ExportToText(); });

			Settings = settings.ToList();
			foreach (var s in Settings)
			{
				s.IsMark = true;
				if (string.IsNullOrEmpty(s.Name))
					s.Name = GetItemName(s.Title);
			}
		}

		public override string ActionTitle => "Экспорт";

		public string ExportToText()
		{
			SettingsText = GetSettingsText();
			return SettingsText;
		}

		//В .net 3.5 нет нативного json сериалайзера...
		private string GetSettingsText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string row in Settings.Where(s => s.IsMark).Select(i => $"{i.Title}{SEPARATOR}{i.Value}"))
				stringBuilder.AppendLine(row);
			return stringBuilder.ToString();
		}
	}
}
