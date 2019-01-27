using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HnHMapSendTool.Core
{
	interface ISender
	{
		/// <summary>
		/// Отправляет сессию
		/// </summary>
		/// <param name="package">Запакованная сессия для отправки</param>
		/// <param name="packageName">Имя сессии/пакета</param>
		/// <returns>Ответ получателя</returns>
		string Send(Stream package, string packageName);
	}
}
