package com.disney.dcpi.swrveutils;

import android.os.Build;
import java.io.File;

public class RootCheckUtil {
  private static final String[] SU_PATHS = {
    "/system/app/Superuser.apk",
    "/sbin/su",
    "/system/bin/su",
    "/system/xbin/su",
    "/data/local/xbin/su",
    "/data/local/bin/su",
    "/system/sd/xbin/su",
    "/system/bin/failsafe/su",
    "/data/local/su",
    "/su/bin/su"
  };

  public static boolean isDeviceRooted() {
    return checkTags() || checkApps() || checkPath();
  }

  private static boolean checkTags() {
    String tags = Build.TAGS;
    return tags != null && tags.contains("test-keys");
  }

  private static boolean checkApps() {
    for (String path : SU_PATHS) {
      if (new File(path).exists()) {
        return true;
      }
    }
    return false;
  }

  private static boolean checkPath() {
    String path = System.getenv("PATH");
    if (path == null) {
      return false;
    }
    for (String dir : path.split(":")) {
      if (!dir.isEmpty() && new File(dir, "su").exists()) {
        return true;
      }
    }
    return false;
  }
}
