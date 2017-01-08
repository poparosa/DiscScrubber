using NLog;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DiscScrubber
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private string _deviceToClean = "C:";

		public MainWindow()
		{
			InitializeComponent();
			PutLogoAndBlurb();
			PopulatePullDown();
			CacheInitialFreeStorage();
			ShowCurrentUsage();
			StartUpdateThread();
			logger.Info("starting up application");
		}


		private void PopulatePullDown()
		{
			var drives = ListDrives.GetDrives();
			var systemDrive = Environment.SystemDirectory;
			foreach (var drive in drives)
			{
				var item = new ComboBoxItem();
				item.Content = drive.Name.ToString();
				DeviceList.Items.Add(item);
			}
			DeviceList.SelectedIndex = 0;
		}

		private void PutLogoAndBlurb()
		{
			Blurb.Text = "Use this application to overwrite free space on the selected device";
			var uriSource = new Uri(@"/DiscScrubber;component/Images/icon-96-xhdpi.png", UriKind.Relative);
			Logo.Source = new BitmapImage(uriSource);
		}

		private void Start_Click(object sender, RoutedEventArgs e)
		{
			ScrubbingThread.ToggleCleaner(_deviceToClean);
			if (Start.Content.ToString() == "Start")
			{ Start.Content = "Stop"; }
			else
			{ Start.Content = "Start"; }
			CacheInitialFreeStorage();
		}

		private void Erase_Click(object sender, RoutedEventArgs e)
		{
			ScrubbingThread.EraseAllTempFiles(_deviceToClean);
			CacheInitialFreeStorage();
		}

		private void DeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			_deviceToClean = (DeviceList.SelectedItem as ComboBoxItem)
				.Content.ToString();
			CacheInitialFreeStorage();
			ShowCurrentUsage();
		}

		#region UpdateThreadMethods

		public void StartUpdateThread()
		{
			Thread t = new Thread(UpdateWorkerThread);
			t.IsBackground = true;
			t.Start();
		}

		private void UpdateWorkerThread()
		{
			while (true)
			{
				UpdateTheStatusBar();
				Thread.Sleep(2000);
			}
		}

		private void UpdateTheStatusBar()
		{
			try
			{ Dispatcher.Invoke((Action)delegate () { ShowCurrentUsage(); }); }
			catch (Exception ex)
			{ logger.Error(ex); }
		}
		#endregion

	}
}
