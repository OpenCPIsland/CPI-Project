package com.disney.nativeaccessibility;

import android.app.Activity;
import android.content.Context;
import android.graphics.Color;
import android.provider.Settings;
import android.text.TextUtils;
import android.view.View;
import android.view.ViewGroup;
import android.view.accessibility.AccessibilityEvent;
import android.view.accessibility.AccessibilityManager;
import android.view.accessibility.AccessibilityNodeInfo;
import android.widget.Button;
import android.widget.TextView;
import com.unity3d.player.UnityPlayer;
import java.util.ArrayList;
import java.util.List;

public class NativeAccessibility {
  private static final String TAG_PREFIX = "AccessibilityButton_";
  private static final String SERVICES = Settings.Secure.ENABLED_ACCESSIBILITY_SERVICES;

  private static final View.AccessibilityDelegate PLAIN_DELEGATE = new View.AccessibilityDelegate() {
    @Override
    public void onInitializeAccessibilityNodeInfo(View host, AccessibilityNodeInfo info) {
      super.onInitializeAccessibilityNodeInfo(host, info);
      info.setEnabled(host.isEnabled());
      info.setClassName("");
    }
  };

  private static Activity activity() {
    return UnityPlayer.currentActivity;
  }

  private static ViewGroup content() {
    Activity activity = activity();
    if (activity == null) {
      return null;
    }
    return activity.getWindow().getDecorView().findViewById(android.R.id.content);
  }

  private static void runOnUi(Runnable runnable) {
    Activity activity = activity();
    if (activity != null) {
      activity.runOnUiThread(runnable);
    }
  }

  private static boolean isAccessibilityTag(View view) {
    Object tag = view.getTag();
    return tag != null && tag.toString().startsWith("Accessibility");
  }

  private static void hide(View view) {
    view.setImportantForAccessibility(View.IMPORTANT_FOR_ACCESSIBILITY_NO);
    view.setFocusable(false);
    view.setContentDescription("?");
  }

  private static void FixParents() {
    ViewGroup root = content();
    if (root == null) {
      return;
    }
    hide(root);
    if (root.getParent() instanceof View) {
      hide((View) root.getParent());
    }
    for (int i = 0; i < root.getChildCount(); i++) {
      View child = root.getChildAt(i);
      if (isAccessibilityTag(child)) {
        continue;
      }
      hide(child);
      if (child instanceof ViewGroup) {
        ViewGroup group = (ViewGroup) child;
        for (int j = 0; j < group.getChildCount(); j++) {
          View inner = group.getChildAt(j);
          if (!isAccessibilityTag(inner)) {
            hide(inner);
          }
        }
      }
    }
  }

  private static boolean IsEnabled(String setting, String service) {
    Activity activity = activity();
    if (activity == null) {
      return false;
    }
    if (Settings.Secure.getInt(activity.getContentResolver(), Settings.Secure.ACCESSIBILITY_ENABLED, 0) != 1) {
      return false;
    }
    String value = Settings.Secure.getString(activity.getContentResolver(), setting);
    if (value == null) {
      return false;
    }
    TextUtils.SimpleStringSplitter splitter = new TextUtils.SimpleStringSplitter(':');
    splitter.setString(value);
    while (splitter.hasNext()) {
      if (splitter.next().contains(service)) {
        return true;
      }
    }
    return false;
  }

  public static int GetAccessibilityLevel() {
    if (IsEnabled(SERVICES, "SwitchAccessService")) {
      return 1;
    }
    if (IsEnabled(SERVICES, "SwitchControlService")) {
      return 2;
    }
    if (IsEnabled(SERVICES, "TalkBackService")) {
      return 10;
    }
    return 0;
  }

  public static boolean IsSwitchControlEnabled() {
    return IsEnabled(SERVICES, "SwitchAccessService") || IsEnabled(SERVICES, "SwitchControlService");
  }

  public static boolean IsVoiceOverEnabled() {
    return IsEnabled(SERVICES, "TalkBackService");
  }

  public static void RenderText(final int id, final int x, final int y, final int width, final int height, final String label) {
    if (width <= 0 || height <= 0) {
      return;
    }
    runOnUi(() -> {
      FixParents();
      ViewGroup root = content();
      if (root == null) {
        return;
      }
      removeTagged(root, id);
      TextView view = new TextView(activity());
      view.setContentDescription(label);
      view.setX(x);
      view.setY(y);
      view.setTextColor(Color.TRANSPARENT);
      view.setAccessibilityDelegate(PLAIN_DELEGATE);
      view.setTag(TAG_PREFIX + id);
      view.setLayoutParams(new ViewGroup.LayoutParams(width, height));
      root.addView(view);
    });
  }

  public static void RenderButton(final int id, final int x, final int y, final int width, final int height, final String label) {
    if (width <= 0 || height <= 0) {
      return;
    }
    runOnUi(() -> {
      FixParents();
      ViewGroup root = content();
      if (root == null) {
        return;
      }
      removeTagged(root, id);
      Button view = new Button(activity());
      view.setBackgroundColor(Color.argb(0, 255, 0, 0));
      view.setTextColor(Color.TRANSPARENT);
      view.setContentDescription(label);
      view.setX(x);
      view.setY(y);
      view.setTag(TAG_PREFIX + id);
      view.setOnClickListener(v -> UnityPlayer.UnitySendMessage("AccessibilityManager", "ButtonClicked", Integer.toString(id)));
      view.setAccessibilityDelegate(PLAIN_DELEGATE);
      view.setLayoutParams(new ViewGroup.LayoutParams(width, height));
      root.addView(view);
    });
  }

  public static void UpdateView(final int id, final int x, final int y, final int width, final int height, final String label) {
    runOnUi(() -> {
      ViewGroup root = content();
      if (root == null) {
        return;
      }
      View view = root.findViewWithTag(TAG_PREFIX + id);
      if (view == null) {
        return;
      }
      view.setX(x);
      view.setY(y);
      ViewGroup.LayoutParams params = view.getLayoutParams();
      params.width = width;
      params.height = height;
      view.setLayoutParams(params);
      view.setContentDescription(label);
    });
  }

  public static void RemoveView(final int id) {
    runOnUi(() -> {
      ViewGroup root = content();
      if (root != null) {
        removeTagged(root, id);
      }
    });
  }

  public static void ClearAllElements() {
    runOnUi(() -> {
      ViewGroup root = content();
      if (root == null) {
        return;
      }
      List<View> remove = new ArrayList<>();
      for (int i = 0; i < root.getChildCount(); i++) {
        View child = root.getChildAt(i);
        Object tag = child.getTag();
        if (tag instanceof String && ((String) tag).contains("Accessibility")) {
          remove.add(child);
        }
      }
      for (View view : remove) {
        root.removeView(view);
      }
    });
  }

  public static void SelectElement(final int id) {
    runOnUi(() -> {
      ViewGroup root = content();
      if (root == null) {
        return;
      }
      View view = root.findViewWithTag(TAG_PREFIX + id);
      if (view == null) {
        return;
      }
      view.setFocusableInTouchMode(true);
      view.setFocusable(true);
      view.requestFocus();
      view.requestFocusFromTouch();
      AccessibilityManager manager = (AccessibilityManager) root.getContext().getSystemService(Context.ACCESSIBILITY_SERVICE);
      if (manager != null && manager.isEnabled()) {
        view.sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_FOCUSED);
        root.sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_ACCESSIBILITY_FOCUS_CLEARED);
        view.sendAccessibilityEvent(AccessibilityEvent.TYPE_VIEW_ACCESSIBILITY_FOCUSED);
      }
    });
  }

  private static void removeTagged(ViewGroup root, int id) {
    View existing = root.findViewWithTag(TAG_PREFIX + id);
    if (existing != null) {
      root.removeView(existing);
    }
  }
}
