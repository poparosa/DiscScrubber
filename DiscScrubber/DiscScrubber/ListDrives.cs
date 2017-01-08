using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscScrubber
{
	public static class ListDrives
	{
		/// <summary>
		/// Loads all Drives of the Computer and returns a List.
		/// </summary>
		public static List<DriveInfo> GetDrives()
		{
			var drives = new List<DriveInfo>();
			foreach (DriveInfo drive in DriveInfo.GetDrives())
			{
				if (drive.IsReady)
				{
					drives.Add(drive);
				}
			}
			return drives;
		}
	}
}
