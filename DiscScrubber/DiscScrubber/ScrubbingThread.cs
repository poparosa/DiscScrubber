using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscScrubber
{
	public static class ScrubbingThread
	{
		private static bool _threadRunning = false;
		private static string _folderToPutTempFiles = @"C:\DiscScrubber";

		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static void EraseAllTempFiles(string device)
		{
			CacheDevice(device);
			try { Directory.Delete(_folderToPutTempFiles, true); }
			catch (Exception ex) { logger.Error(ex); }
		}

		public static void ToggleCleaner(string device)
		{
			CacheDevice(device);
			if (_threadRunning)
			{ Stop(); }
			else
			{ Start(); }
		}

		public static void Start()
		{
			if (!_threadRunning)
			{
				_threadRunning = true;
				Thread t = new Thread(WriteToDrive);
				t.IsBackground = true;
				t.Start();
			}
		}

		private static void WriteToDrive()
		{
			int counter = 0;
			while (_threadRunning)
			{ WriteTempFile(counter++); }
		}

		private static void WriteTempFile(int Counter)
		{
			var level0 = Counter / 100;
			var level1 = level0 / 100;
			var targetFolder = Path.Combine(_folderToPutTempFiles, level1.ToString(), level0.ToString());
			Directory.CreateDirectory(targetFolder);
			var targetFullPath = Path.Combine(targetFolder, (Counter % 100).ToString() + ".txt");
			if (!File.Exists(targetFullPath))
			{
				try { WriteTheBits(targetFullPath); }
				catch (Exception ex)
				{
					logger.Error("cannot write to ", targetFolder);
					logger.Error(ex);
					Thread.Sleep(500);
				}
			}
		}

		private static void WriteTheBits(string targetFullPath)
		{
			var sourceFullPath = @"C:\Windows\System32\cmd.exe";
			System.IO.File.Copy(sourceFullPath, targetFullPath);
		}

		public static void Stop()
		{ _threadRunning = false; }

		private static void CacheDevice(string device)
		{
			string leaningFolder = @"{0}DiscScrubber";
			_folderToPutTempFiles = string.Format(leaningFolder, device);
		}
	}
}
