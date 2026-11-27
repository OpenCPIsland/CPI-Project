package com.cpisland.unityplugins;

import android.Manifest;
import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentManager;
import android.content.pm.PackageManager;
import android.os.Build;
import android.util.Log;
import com.unity3d.player.UnityPlayer;

public class PermissionGranter {
  private static final String TAG = "PermissionGranter";
  private static final String PERMISSION_GRANTED = "PERMISSION_GRANTED";
  private static final String PERMISSION_DENIED = "PERMISSION_DENIED";
  private static final String PERMISSION_NEVER = "PERMISSION_NEVER";
  private static final String UNITY_CALLBACK_GAMEOBJECT_NAME = "Initializers";
  private static final String UNITY_CALLBACK_METHOD_NAME = "permissionRequestCallbackInternal";
  private static final int REQUEST_CODE = 0x5047;

  public static String getPermissionStringFromEnumInt(int permissionEnum) throws Exception {
    if (permissionEnum == 0) {
      return Manifest.permission.READ_EXTERNAL_STORAGE;
    }
    Log.e(TAG, "Error. Unknown permissionEnum");
    throw new Exception(String.format("Error. Unknown permissionEnum %d", permissionEnum));
  }

  private static void send(String result) {
    UnityPlayer.UnitySendMessage(UNITY_CALLBACK_GAMEOBJECT_NAME, UNITY_CALLBACK_METHOD_NAME, result);
  }

  public static void grantPermission(Activity activity, int permissionEnum) {
    final String permission;
    try {
      permission = getPermissionStringFromEnumInt(permissionEnum);
    } catch (Exception e) {
      Log.w(TAG, String.format("Unable to request permission: %s", e.getMessage()));
      send(PERMISSION_DENIED);
      return;
    }
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU && Manifest.permission.READ_EXTERNAL_STORAGE.equals(permission)) {
      send(PERMISSION_GRANTED);
      return;
    }
    if (activity.checkSelfPermission(permission) == PackageManager.PERMISSION_GRANTED) {
      send(PERMISSION_GRANTED);
      return;
    }
    FragmentManager manager = activity.getFragmentManager();
    RequestFragment fragment = new RequestFragment();
    fragment.permission = permission;
    manager.beginTransaction().add(0, fragment).commit();
  }

  public static class RequestFragment extends Fragment {
    String permission;
    boolean requested;

    @Override
    public void onStart() {
      super.onStart();
      if (permission == null) {
        getFragmentManager().beginTransaction().remove(this).commit();
        return;
      }
      if (requested) {
        return;
      }
      requested = true;
      requestPermissions(new String[] { permission }, REQUEST_CODE);
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
      if (requestCode != REQUEST_CODE) {
        return;
      }
      if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
        send(PERMISSION_GRANTED);
      } else if (shouldShowRequestPermissionRationale(permission)) {
        send(PERMISSION_DENIED);
      } else {
        send(PERMISSION_NEVER);
      }
      permission = null;
      getFragmentManager().beginTransaction().remove(this).commit();
    }
  }
}
