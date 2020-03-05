using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core.SettingImportExport
{
	public class SettingImportViewModel : SIEBaseViewModel
	{
		public SettingImportViewModel()
		{
			IsSettingsTextReadOnly = false;
			DoCommand = new RelayCommand(() => { IsNeedImport = true; OnWorkDone?.Invoke(); });
		}

		private bool _isNeedImport;
		public bool IsNeedImport
		{
			get { return _isNeedImport; }
			private set
			{
				_isNeedImport = value;
				OnPropertyChanged(nameof(IsNeedImport));
			}
		}

		public override string ActionTitle => "Импорт";

		public void ReadText()
		{
			this.Settings = ConvertText(SettingsText).ToList();
			OnPropertyChanged(nameof(Settings));
		}

		private IEnumerable<SettingItem> ConvertText(string input)
		{
			var rows = input.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
			foreach (string row in rows)
			{
				var r = row.Trim();
				if (!r.Contains(SEPARATOR))
					continue;

				var sIndex = r.IndexOf(SEPARATOR);
				yield return new SettingItem()
				{
					IsMark = true,
					Title = r.Substring(0, sIndex),
					Value = r.Substring(sIndex + 1, r.Length - sIndex - 1),
					Name = GetItemName(r.Substring(0, sIndex))
				};
			}
		}

		protected override void OnSettingsTextChanged()
		{
			base.OnSettingsTextChanged();
			ReadText();
		}
	}
}
