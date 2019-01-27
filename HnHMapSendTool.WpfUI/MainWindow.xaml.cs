using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace HnHMapSendTool.WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		private const int MAX_MESSEGES_IN_OUTPUT = 1000;

		private WinForms.NotifyIcon _notifier;
		private WindowState _lastWindowState;
		private Core.SendToolViewModel _sendToolViewModel;

		public MainWindow()
        {
            InitializeComponent();

			_sendToolViewModel = new Core.SendToolViewModel(ProcessError, LogMessage);
			DataContext = _sendToolViewModel;

			/*if (!String.IsNullOrEmpty(_sendToolViewModel.SessionsDirectory) && !String.IsNullOrEmpty(_sendToolViewModel.Url))
				this.WindowState = WindowState.Minimized;*/

			_lastWindowState = this.WindowState;

			_notifier = new WinForms.NotifyIcon();
			_notifier.MouseDown += Notifier_MouseDown;
			_notifier.MouseDoubleClick += Notifier_MouseDoubleClick;
			_notifier.Text = "HnHMapSendTool";
			_notifier.Icon = Properties.Resources.MapIcon;
			_notifier.Visible = true;
		}

		private void Notifier_MouseDoubleClick(object sender, WinForms.MouseEventArgs e)
		{
			ShowHideMainWindow();
		}

		private void Notifier_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == WinForms.MouseButtons.Right)
			{
				ContextMenu menu = (ContextMenu)this.FindResource("NotifierContextMenu");
				menu.IsOpen = true;
			}
		}

		private void OpenMainWindowMenuItem_Click(object sender, RoutedEventArgs e)
		{
			ShowHideMainWindow();
		}

		private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void Window_StateChanged(object sender, EventArgs e)
		{
			if (WindowState == WindowState.Minimized)
			{
				this.Hide();
			}
			else
			{
				_lastWindowState = this.WindowState;
			}
		}

		private void ShowHideMainWindow()
		{
			if (WindowState == WindowState.Minimized)
			{
				Show();
				WindowState = _lastWindowState;
				this.Focus();
			}
			else
			{
				WindowState = WindowState.Minimized;
			}
		}

		private void MinimizedButton_Click(object sender, RoutedEventArgs e)
		{
			ShowHideMainWindow();
		}

		private void LogMessage(string message)
		{
			LogMessage(message, false);
		}

		private void LogMessage(string message, bool isWarning)
		{
			Dispatcher.Invoke((Action)delegate
			{
				lock (OutputListBox)
				{
					if (OutputListBox.Items.Count > MAX_MESSEGES_IN_OUTPUT)
						OutputListBox.Items.RemoveAt(0);
					OutputElement outputElement = new OutputElement(message, isWarning);
					OutputListBox.Items.Add(outputElement);
					OutputListBox.Items.MoveCurrentToLast();
					OutputListBox.ScrollIntoView(outputElement);
				}
			});
		}

		private void ProcessError(Exception ex)
		{
			App.LogError(ex);
			LogMessage(ex.Message, true);
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void SendAllMenuItem_Click(object sender, RoutedEventArgs e)
		{
			_sendToolViewModel.SendAllNewSessions();
		}

		private void SessionsDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			var dir = GetDirectoryName(_sendToolViewModel.SessionsDirectory);
			if (!String.IsNullOrEmpty(dir))
				_sendToolViewModel.SessionsDirectory = dir;
		}

		private void MoveDirectoryButton_Click(object sender, RoutedEventArgs e)
		{
			var dir = GetDirectoryName(_sendToolViewModel.MoveDirectory);
			if (!String.IsNullOrEmpty(dir))
				_sendToolViewModel.MoveDirectory = dir;
		}

		private string GetDirectoryName(string currentDirectory)
		{
			var folderBrowserDialog = new WinForms.FolderBrowserDialog();
			folderBrowserDialog.SelectedPath = currentDirectory;
			var result = folderBrowserDialog.ShowDialog();
			if (result == WinForms.DialogResult.OK)
				return folderBrowserDialog.SelectedPath;
			else
				return string.Empty;
		}
	}
}
