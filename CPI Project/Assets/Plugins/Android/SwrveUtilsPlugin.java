package com.disney.dcpi.swrveutils;

import android.app.Activity;
import android.content.Context;
import android.util.Log;
import java.util.HashMap;
import java.util.Map;

public class SwrveUtilsPlugin {
  private static final String TAG = "SwrveUtils";
  private static final String DEFAULT_GIDA = "00000000-0000-0000-0000-000000000000";
  private static final int LAT_ENABLED = 1;

  private static Context _context;
  private static SwrveUtilsPlugin instance;

  private boolean debugLogging;

  public SwrveUtilsPlugin(Context context) {
    _context = context;
  }

  public static void setContext(Activity activity) {
    setContext(activity != null ? activity.getApplicationContext() : null);
  }

  public static void setContext(Context context) {
    if (context != null) {
      _context = context.getApplicationContext();
    }
  }

  public static synchronized SwrveUtilsPlugin instance() {
    if (instance == null) {
      instance = new SwrveUtilsPlugin(_context);
    }
    return instance;
  }

  public static Map<String, Object> getUtilsInfo() {
    Map<String, Object> info = new HashMap<>();
    info.put("gida", DEFAULT_GIDA);
    info.put("is_lat", LAT_ENABLED);
    return info;
  }

  public HashMap<String, Object> getAdInfo() {
    return new HashMap<>(getUtilsInfo());
  }

  public boolean get() {
    return true;
  }

  public String gida() {
    log("[GIDA] retValue from gida(): " + DEFAULT_GIDA);
    return DEFAULT_GIDA;
  }

  public int isLat() {
    return LAT_ENABLED;
  }

  public int isJailBroken() {
    int value = RootCheckUtil.isDeviceRooted() ? 1 : 0;
    log("[isJailBroken] retValue is: " + value);
    return value;
  }

  public void setDebugLogging(boolean enabled) {
    debugLogging = enabled;
  }

  private void log(String message) {
    if (debugLogging) {
      Log.d(TAG, message);
    }
  }
}
