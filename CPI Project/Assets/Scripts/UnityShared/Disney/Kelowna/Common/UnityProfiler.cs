using Tweaker.Core;
using UnityEngine.Profiling;

namespace Disney.Kelowna.Common
{
	public static class UnityProfiler
	{
		[Tweakable("Debug.Performance.Profiler.Enabled")]
		public static bool ProfilerEnabled
		{
			get
			{
				return Profiler.enabled;
			}
			set
			{
				Profiler.enabled = value;
			}
		}

		[Invokable("Debug.Performance.Profiler.SetLogFile")]
		public static void SetLog(string file = "ProfilerData.log", bool enableBinaryLog = false, bool enableProfilerNow = false)
		{
			Profiler.enableBinaryLog = enableBinaryLog;
			Profiler.logFile = file;
			if (enableProfilerNow)
			{
				ProfilerEnabled = true;
			}
		}
	}
}
