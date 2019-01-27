using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.WpfUI
{
    internal class OutputElement
    {
		public OutputElement(string outputText, bool isWarning)
		{
			OutputDateTime = DateTime.Now;
			OutputText = outputText;
			IsWarning = isWarning;
		}

		public DateTime OutputDateTime { get; }
		public string OutputText { get; }
		public bool IsWarning { get; }
		public string FullText { get { return $"{OutputDateTime} - {OutputText}"; } }

		public override string ToString()
		{
			return FullText;
		}
	}
}
