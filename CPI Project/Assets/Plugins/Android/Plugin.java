package com.disney.mobilenetwork.utils;

import android.app.Activity;
import android.app.Application;
import com.unity3d.player.UnityPlayer;

public class Plugin {
  protected Activity mActivity;
  protected Application mApplication;

  protected Plugin() {
    attach();
  }

  protected final void attach() {
    Activity activity = UnityPlayer.currentActivity;
    if (activity != null) {
      mActivity = activity;
      mApplication = activity.getApplication();
    }
  }

  public Activity getActivity() {
    Activity current = UnityPlayer.currentActivity;
    if (current != null && current != mActivity) {
      mActivity = current;
      mApplication = current.getApplication();
    }
    return mActivity;
  }

  public Application getApplication() {
    if (mApplication == null) {
      getActivity();
    }
    return mApplication;
  }
}
