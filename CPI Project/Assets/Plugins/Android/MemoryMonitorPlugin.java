package com.disney.mobilenetwork.plugins;

import android.app.ActivityManager;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.BatteryManager;
import android.os.Debug;
import android.os.Environment;
import android.os.StatFs;
import com.disney.mobilenetwork.utils.Plugin;

public class MemoryMonitorPlugin extends Plugin {
  public static MemoryMonitorPlugin mInstance = null;

  public static synchronized MemoryMonitorPlugin getInstance() {
    if (mInstance == null) {
      mInstance = new MemoryMonitorPlugin();
    }
    return mInstance;
  }

  public float GetBatteryPercent() {
    Context context = getApplication();
    IntentFilter filter = new IntentFilter(Intent.ACTION_BATTERY_CHANGED);
    Intent battery = context.registerReceiver(null, filter);
    if (battery == null) {
      BatteryManager manager = (BatteryManager) context.getSystemService(Context.BATTERY_SERVICE);
      return manager != null ? manager.getIntProperty(BatteryManager.BATTERY_PROPERTY_CAPACITY) / 100f : -1f;
    }
    int level = battery.getIntExtra(BatteryManager.EXTRA_LEVEL, -1);
    int scale = battery.getIntExtra(BatteryManager.EXTRA_SCALE, -1);
    return (float) level / (float) scale;
  }

  public long GetFreeBytes() {
    return new StatFs(Environment.getDataDirectory().getAbsolutePath()).getFreeBytes();
  }

  public long GetTotalBytes() {
    return new StatFs(Environment.getDataDirectory().getAbsolutePath()).getTotalBytes();
  }

  public int GetHeapSize() {
    ActivityManager manager = (ActivityManager) getApplication().getSystemService(Context.ACTIVITY_SERVICE);
    return manager.getMemoryClass();
  }

  public long GetProcessUsedBytes() {
    Debug.MemoryInfo info = new Debug.MemoryInfo();
    Debug.getMemoryInfo(info);
    return (long) info.getTotalPss() * 1024L;
  }
}
