using Disney.LaunchPadFramework;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Profiling;

namespace Disney.MobileNetwork
{
    public class MemoryMonitorWindowsManager : MemoryMonitorManager
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        public static bool Enabled = true;

        [DllImport("MemoryMonitorWindows")]
        private static extern ulong _getProcessUsedBytes();

        [DllImport("MemoryMonitorWindows")]
        private static extern ulong _getTotalBytes();

        [DllImport("MemoryMonitorWindows")]
        private static extern ulong _getFreeBytes();

        [DllImport("MemoryMonitorWindows")]
        private static extern float _getBatteryPercent();

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

        public override ulong GetTotalBytes()
        {
            return _getTotalBytes();
        }

        public override ulong GetFreeBytes()
        {
            return _getFreeBytes();
        }

        public override float GetBatteryPercent()
        {
            return _getBatteryPercent();
        }

#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        public static bool Enabled = true;

        [DllImport("libMemoryMonitorLinux", EntryPoint = "get_process_used_bytes")]
        private static extern ulong _getProcessUsedBytes();

        [DllImport("libMemoryMonitorLinux", EntryPoint = "get_total_bytes")]
        private static extern ulong _getTotalBytes();

        [DllImport("libMemoryMonitorLinux", EntryPoint = "get_free_bytes")]
        private static extern ulong _getFreeBytes();

        [DllImport("libMemoryMonitorLinux", EntryPoint = "get_battery_percent")]
        private static extern float _getBatteryPercent();

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

        public override ulong GetTotalBytes()
        {
            return _getTotalBytes();
        }

        public override ulong GetFreeBytes()
        {
            return _getFreeBytes();
        }

        public override float GetBatteryPercent()
        {
            return _getBatteryPercent();
        }

#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX_ARM || UNITY_EDITOR_OSX_ARM
        public static bool Enabled = true;

        [DllImport("libMemoryMonitorOSX", EntryPoint = "get_process_used_bytes")]
        private static extern ulong _getProcessUsedBytes();

        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetTotalBytes")]
        private static extern ulong _getTotalBytes();

        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetFreeBytes")]
        private static extern ulong _getFreeBytes();

        [DllImport("libMemoryMonitorOSX", EntryPoint = "_GetBatteryPercent")]
        private static extern float _getBatteryPercent();

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

        public override ulong GetTotalBytes()
        {
            return _getTotalBytes();
        }

        public override ulong GetFreeBytes()
        {
            return _getFreeBytes();
        }

        public override float GetBatteryPercent()
        {
            return _getBatteryPercent();
        }

#elif UNITY_WEBGL
        public static bool Enabled = true;

        [DllImport("__Internal")]
        private static extern double MemoryMonitorWebGL_GetWasmHeapSize();

        [DllImport("__Internal")]
        private static extern double MemoryMonitorWebGL_GetWasmHeapMax();

        [DllImport("__Internal")]
        private static extern double MemoryMonitorWebGL_GetJsHeapUsed();

        [DllImport("__Internal")]
        private static extern double MemoryMonitorWebGL_GetJsHeapTotal();

        [DllImport("__Internal")]
        private static extern double MemoryMonitorWebGL_GetJsHeapLimit();

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
            if (!Enabled)
            {
                return base.GetProcessUsedBytes();
            }

            try
            {
                double wasmBytes = MemoryMonitorWebGL_GetWasmHeapSize();

                if (wasmBytes < 0)
                {
                    wasmBytes = 0;
                }

                double jsUsed = MemoryMonitorWebGL_GetJsHeapUsed();

                if (jsUsed < 0)
                {
                    jsUsed = 0;
                }

                double sum = wasmBytes + jsUsed;

                if (sum <= 0)
                {
                    return (ulong)Profiler.usedHeapSizeLong;
                }

                if (sum >= ulong.MaxValue)
                {
                    return ulong.MaxValue;
                }

                return (ulong)sum;
            }
            catch
            {
                Enabled = false;
                return base.GetProcessUsedBytes();
            }
        }

        public override ulong GetTotalBytes()
        {
            if (!Enabled)
            {
                return base.GetTotalBytes();
            }

            try
            {
                double limit = MemoryMonitorWebGL_GetWasmHeapMax();

                if (limit <= 0)
                {
                    limit = MemoryMonitorWebGL_GetJsHeapLimit();
                }

                if (limit <= 0)
                {
                    limit = MemoryMonitorWebGL_GetJsHeapTotal();
                }

                if (limit <= 0)
                {
                    return 0uL;
                }

                if (limit >= ulong.MaxValue)
                {
                    return ulong.MaxValue;
                }

                return (ulong)limit;
            }
            catch
            {
                return 0uL;
            }
        }

        public override ulong GetFreeBytes()
        {
            ulong total = GetTotalBytes();

            if (total == 0uL)
            {
                return 0uL;
            }

            ulong used;

            try
            {
                double wasmBytes = Enabled ? MemoryMonitorWebGL_GetWasmHeapSize() : 0;
                used = (wasmBytes > 0) ? (ulong)wasmBytes : GetProcessUsedBytes();
            }
            catch
            {
                used = GetProcessUsedBytes();
            }

            if (used >= total)
            {
                return 0uL;
            }

            return total - used;
        }
#endif
    }
}