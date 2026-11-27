package com.disney.mobilenetwork.plugins;

import android.app.Activity;
import android.content.Context;
import android.graphics.Insets;
import android.graphics.Point;
import android.graphics.Rect;
import android.media.AudioManager;
import android.os.Build;
import android.speech.tts.TextToSpeech;
import android.util.DisplayMetrics;
import android.view.Display;
import android.view.WindowInsets;
import android.view.WindowMetrics;
import android.view.accessibility.AccessibilityEvent;
import android.view.accessibility.AccessibilityManager;
import com.disney.mobilenetwork.utils.Plugin;
import com.disney.nativeaccessibility.NativeAccessibility;
import java.util.Locale;

public class AccessibilityPlugin extends Plugin implements TextToSpeech.OnInitListener {
  public static AccessibilityPlugin mInstance = null;

  private TextToSpeech mSpeakApi = null;
  private volatile boolean mApiReady = false;
  private String mPendingText = null;
  private float mPendingRate = 0.5f;

  public static synchronized AccessibilityPlugin getInstance() {
    if (mInstance == null) {
      mInstance = new AccessibilityPlugin();
    }
    return mInstance;
  }

  public float GetAdjustedFontSize(float fontSize) {
    return getActivity().getResources().getConfiguration().fontScale * fontSize;
  }

  public int GetFullScreenWidth() {
    return fullBounds().width();
  }

  public int GetFullScreenHeight() {
    return fullBounds().height();
  }

  public int GetScreenWidth() {
    return usableSize().x;
  }

  public int GetScreenHeight() {
    return usableSize().y;
  }

  public int GetStatusBarHeight() {
    Activity activity = getActivity();
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
      WindowInsets insets = activity.getWindowManager().getCurrentWindowMetrics().getWindowInsets();
      return insets.getInsetsIgnoringVisibility(WindowInsets.Type.statusBars()).top;
    }
    int id = activity.getResources().getIdentifier("status_bar_height", "dimen", "android");
    return id > 0 ? activity.getResources().getDimensionPixelSize(id) : 0;
  }

  public boolean IsOtherAudioPlaying() {
    AudioManager audio = (AudioManager) getActivity().getSystemService(Context.AUDIO_SERVICE);
    return audio != null && audio.isMusicActive();
  }

  public void Speak(String text, float rate) {
    Activity activity = getActivity();
    AccessibilityManager manager = (AccessibilityManager) activity.getSystemService(Context.ACCESSIBILITY_SERVICE);
    if (NativeAccessibility.GetAccessibilityLevel() > 0 && manager != null && manager.isEnabled()) {
      AccessibilityEvent event;
      if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
        event = new AccessibilityEvent(AccessibilityEvent.TYPE_ANNOUNCEMENT);
      } else {
        event = AccessibilityEvent.obtain(AccessibilityEvent.TYPE_ANNOUNCEMENT);
      }
      event.setClassName(getClass().getName());
      event.setPackageName(activity.getPackageName());
      event.getText().add(text);
      manager.sendAccessibilityEvent(event);
      return;
    }
    if (mSpeakApi == null) {
      if (mPendingText == null) {
        mPendingText = text;
        mPendingRate = rate;
        mSpeakApi = new TextToSpeech(activity.getApplicationContext(), this);
      }
      return;
    }
    if (mApiReady) {
      mSpeakApi.setSpeechRate(rate);
      mSpeakApi.speak(text, TextToSpeech.QUEUE_FLUSH, null, "AccessibilityPlugin");
    }
  }

  @Override
  public void onInit(int status) {
    if (status != TextToSpeech.SUCCESS) {
      return;
    }
    mApiReady = true;
    mSpeakApi.setLanguage(Locale.US);
    String text = mPendingText;
    float rate = mPendingRate;
    mPendingText = null;
    if (text != null) {
      Speak(text, rate);
    }
  }

  private Rect fullBounds() {
    Activity activity = getActivity();
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
      return activity.getWindowManager().getMaximumWindowMetrics().getBounds();
    }
    DisplayMetrics metrics = new DisplayMetrics();
    activity.getWindowManager().getDefaultDisplay().getRealMetrics(metrics);
    return new Rect(0, 0, metrics.widthPixels, metrics.heightPixels);
  }

  private Point usableSize() {
    Activity activity = getActivity();
    if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) {
      WindowMetrics metrics = activity.getWindowManager().getCurrentWindowMetrics();
      Insets insets = metrics.getWindowInsets().getInsetsIgnoringVisibility(WindowInsets.Type.navigationBars() | WindowInsets.Type.displayCutout());
      Rect bounds = metrics.getBounds();
      return new Point(bounds.width() - insets.left - insets.right, bounds.height() - insets.top - insets.bottom);
    }
    Display display = activity.getWindowManager().getDefaultDisplay();
    Point size = new Point();
    display.getSize(size);
    return size;
  }
}
