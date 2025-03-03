using Disney.LaunchPadFramework;
using System;
using System.Runtime.InteropServices;

namespace Disney.MobileNetwork
{
	public class MemoryMonitorWindowsManager : MemoryMonitorManager
	{
#if UNITY_STANDALONE_WIN
		public static bool Enabled = true;

		[DllImport("MemoryMonitorWindows")]
		private static extern ulong _getProcessUsedBytes();

		protected override void Init()
		{
			try
			{
				GetProcessUsedBytes();
			}
			catch (Exception ex)
			{
				Enabled = false;
				Log.LogException(typeof(MemoryMonitorWindowsManager), ex);
			}
		}

		public override ulong GetProcessUsedBytes()
		{
			return Enabled ? _getProcessUsedBytes() : base.GetProcessUsedBytes();
		}
#endif
	}
}
