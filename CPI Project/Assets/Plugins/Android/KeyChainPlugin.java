package com.disney.mobilenetwork.plugins;

import android.app.Application;
import android.content.Context;
import com.disney.mobilenetwork.utils.BuildSettings;
import com.disney.mobilenetwork.utils.Plugin;

public class KeyChainPlugin extends Plugin {
  public static KeyChainPlugin mInstance = null;
  
  private ObscuredSharedPreferences prefs;
  
  public static KeyChainPlugin getInstance() {
    return mInstance;
  }
  
  public void GenerateAndStoreKey() {
    this.prefs = new ObscuredSharedPreferences((Context)this.mActivity, this.mActivity.getSharedPreferences("Store", 0));
    this.prefs.GenerateAndStoreKey();
  }
  
  public String GetString(String paramString) {
    return this.prefs.getString(paramString, "");
  }
  
  public void PutString(String paramString1, String paramString2) {
    this.prefs.edit().putString(paramString1, paramString2).commit();
  }
  
  public void RemoveString(String paramString) {
    this.prefs.edit().remove(paramString);
  }
  
  public void init(Application paramApplication, BuildSettings paramBuildSettings) {
    mInstance = this;
    super.init(paramApplication, paramBuildSettings);
  }
}