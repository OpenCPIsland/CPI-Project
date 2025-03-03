using UnityEngine;
using UnityEngine.Profiling;

namespace Disney.MobileNetwork
{
	public class MemoryMonitorManager : MonoBehaviour
	{
		public void Awake()
		{
			Init();
		}

		protected virtual void Init()
		{
		}

		public virtual ulong GetHeapSize()
		{
			return (ulong)Profiler.usedHeapSizeLong;
		}

		public virtual ulong GetFreeBytes()
		{
			return 0uL;
		}

		public virtual ulong GetTotalBytes()
		{
			return 0uL;
		}

		public virtual float GetBatteryPercent()
		{
			return 1f;
		}

		public virtual ulong GetProcessUsedBytes()
		{
			return 0uL;
		}
	}
}
