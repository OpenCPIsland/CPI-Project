package com.disney.mobilenetwork.plugins;

import android.app.Activity;
import com.disney.mobilenetwork.utils.Plugin;

public class KeyChainPlugin extends Plugin {
  public static KeyChainPlugin mInstance = null;
  
  private ObscuredSharedPreferences prefs;
  
  public static synchronized KeyChainPlugin getInstance() {
    if (mInstance == null) {
      mInstance = new KeyChainPlugin();
    }
    return mInstance;
  }
  
  public void GenerateAndStoreKey() {
    Activity activity = getActivity();
    this.prefs = new ObscuredSharedPreferences(activity, activity.getSharedPreferences("Store", 0));
    this.prefs.GenerateAndStoreKey();
  }
  
  public String GetString(String paramString) {
    return this.prefs.getString(paramString, "");
  }
  
  public void PutString(String paramString1, String paramString2) {
    this.prefs.edit().putString(paramString1, paramString2).commit();
  }
  
  public void RemoveString(String paramString) {
    this.prefs.edit().remove(paramString).commit();
  }
}