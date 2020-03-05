using HnHMapSendTool.Core.SettingImportExport;

namespace HnHMapSendTool.Core
{
	public interface ISendToolViewProvider 
	{
		void ExportSettings(SettingExportViewModel viewModel);
		void ImportSettings(SettingImportViewModel viewModel);
	}
}
