using Disney.MobileNetwork;
using UnityEngine;

namespace Disney.MobileNetwork
{
	public class MemoryMonitorAndroidManager : MemoryMonitorManager
	{
#if UNITY_ANDROID
		private AndroidJavaObject androidPlugin;

		protected override void Init()
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.mobilenetwork.plugins.MemoryMonitorPlugin"))
			{
				if (androidJavaClass != null)
				{
					androidPlugin = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
				}
			}
		}

		public override ulong GetHeapSize()
		{
			int num = androidPlugin.Call<int>("GetHeapSize", new object[0]);
			return (ulong)((long)num * 1024L * 1024);
		}

		public override ulong GetFreeBytes()
		{
			return (ulong)androidPlugin.Call<long>("GetFreeBytes", new object[0]);
		}

		public override ulong GetTotalBytes()
		{
			return (ulong)androidPlugin.Call<long>("GetTotalBytes", new object[0]);
		}

		public override float GetBatteryPercent()
		{
			return androidPlugin.Call<float>("GetBatteryPercent", new object[0]);
		}

		public override ulong GetProcessUsedBytes()
		{
            return (ulong)0;// androidPlugin.Call<long>("GetProcessUsedBytes", new object[0]);
		}
#endif
	}
}
