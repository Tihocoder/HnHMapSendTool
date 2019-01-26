using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core
{
	interface ISender
	{
		void Send(Stream package, string packageName);
	}
}
