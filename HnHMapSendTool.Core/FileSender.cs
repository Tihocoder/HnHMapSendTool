﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core
{
	internal class FileSender : ISender
	{
		private string _outputDirectory;

		public FileSender(string outputDirectory)
		{
			_outputDirectory = outputDirectory.TrimEnd('\\');
		}

		public string Send(Stream package, string packageName)
		{
			string modificator = "";
			int i = 0;
			string outputFileName;

			do
			{
				outputFileName = $"{_outputDirectory}\\{packageName}{modificator}.zip";
				i++;
				modificator = $"({i})";
			}
			while (File.Exists(outputFileName));

			Helper.SaveAsFile(package, outputFileName, FileMode.OpenOrCreate);

			return $"done"; //FIXME: Текстовые константы на уровень интерфейса.
		}
	}
}
