using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core.SettingImportExport
{
	public class SettingItem : NotifyPropertyViewModel
	{
		private bool _isMark;

		public bool IsMark
		{
			get { return _isMark; }
			set
			{
				_isMark = value;
				OnPropertyChanged(nameof(IsMark));
			}
		}

		private string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				OnPropertyChanged(nameof(Title));
			}
		}

		private string _value;
		public string Value
		{
			get { return _value; }
			set
			{
				_value = value;
				OnPropertyChanged(nameof(Value));
			}
		}
	}
}
