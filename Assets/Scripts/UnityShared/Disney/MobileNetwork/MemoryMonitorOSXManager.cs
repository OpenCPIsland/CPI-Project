using Disney.MobileNetwork;
using System.Runtime.InteropServices;

namespace Disney.MobileNetwork
{
	public class MemoryMonitorOSXManager : MemoryMonitorManager
	{
#if UNITY_STANDALONE_OSX
		[DllImport("MemoryMonitorOSX")]
		private static extern ulong _GetHeapSize();

		[DllImport("MemoryMonitorOSX")]
		private static extern ulong _GetFreeBytes();

		[DllImport("MemoryMonitorOSX")]
		private static extern ulong _GetTotalBytes();

		[DllImport("MemoryMonitorOSX")]
		private static extern float _GetBatteryPercent();

		[DllImport("MemoryMonitorOSX")]
		private static extern ulong _GetProcessUsedBytes();

		protected override void Init()
		{
		}

		public override ulong GetHeapSize()
		{
			return _GetHeapSize();
		}

		public override ulong GetFreeBytes()
		{
			return _GetFreeBytes();
		}

		public override ulong GetTotalBytes()
		{
			return _GetTotalBytes();
		}

		public override float GetBatteryPercent()
		{
			return _GetBatteryPercent();
		}

		public override ulong GetProcessUsedBytes()
		{
			return _GetProcessUsedBytes();
		}
#endif
	}
}
