using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiscScrubber
{
	public partial class MainWindow : Window
	{
		Brush used = new SolidColorBrush(System.Windows.Media.Colors.Blue);
		Brush cleaned = new SolidColorBrush(System.Windows.Media.Colors.Yellow);
		Brush open = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue);

		private double _initialFreePercentage;

		private void CacheInitialFreeStorage()
		{
			_initialFreePercentage = GetFreePercentage();
		}

		public void ShowCurrentUsage()
		{
			double percentUsed = 100 - _initialFreePercentage;
			double percentCleaned = _initialFreePercentage - GetFreePercentage() ;
			ShowUsage(percentUsed, percentCleaned);
		}

		private double GetDriveSizeGB()
		{
			DriveInfo drive = new DriveInfo((DeviceList.SelectedItem as ComboBoxItem).Content.ToString());
			var KBytes = drive.TotalSize / 1024.0;
			var MBytes = KBytes / 1024.0;
			var GBytes = MBytes / 1024.0;
			return GBytes;
		}

		private double GetFreePercentage()
		{
			DriveInfo drive = new DriveInfo((DeviceList.SelectedItem as ComboBoxItem).Content.ToString());
			double percentFree = 100 * (double)drive.TotalFreeSpace / drive.TotalSize;
			return percentFree;
		}

		private void ShowUsage(double percentUsed, double percentOverWritten)
		{
			var buttonHeight = 20;
			var fullWidth = ProgessBar.Width;
			var percentFree = 100 - percentUsed - percentOverWritten;
			var usedWidth = fullWidth * (percentUsed / 100.0);
			var overWrittenWidth = fullWidth * (percentOverWritten / 100.0);
			var freeWidth = fullWidth * (percentFree / 100.0);
			if (overWrittenWidth < 0.0)
			{ overWrittenWidth = 0; }
			var usedPart = new Button()
			{
				Width = usedWidth,
				Content = "used",
				ToolTip = GetTooltip("Used", percentUsed),
				Height = buttonHeight,
				Background = used
			};
			var cleanedPart = new Button()
			{
				Width = overWrittenWidth,
				Content = "cleaned",
				ToolTip = GetTooltip("Cleaned", percentOverWritten),
				Height = buttonHeight,
				Background = cleaned
			};
			var freePart = new Button()
			{
				Width = freeWidth,
				Content = "free",
				ToolTip = GetTooltip("Free", percentFree),
				Height = buttonHeight,
				Background = open
			};
			ProgessBar.Children.Clear();
			ProgessBar.Orientation = Orientation.Horizontal;
			ProgessBar.Children.Add(usedPart);
			ProgessBar.Children.Add(cleanedPart);
			ProgessBar.Children.Add(freePart);
			ProgessBar.Height = buttonHeight + 2;
		}

		private object GetTooltip(string description , double percent)
		{
			var totalSizeGB = GetDriveSizeGB();
			var answer = string.Format("{0}: {1:00.00}% Size {2:00.00}GB", 
				description, percent, totalSizeGB / 100.0 * percent);
			return answer;
		}
	}
}
