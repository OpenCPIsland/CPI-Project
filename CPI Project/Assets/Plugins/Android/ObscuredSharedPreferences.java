package com.disney.mobilenetwork.plugins;

import android.content.Context;
import android.content.SharedPreferences;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;

public class ObscuredSharedPreferences implements SharedPreferences {
  protected final Context context;
  protected final SharedPreferences delegate;
  private BaseKeyStore.KeystoreType keystoreType = BaseKeyStore.KeystoreType.NONE;

  public ObscuredSharedPreferences(Context context, SharedPreferences delegate) {
    this.context = context;
    this.delegate = delegate;
  }

  private void SetKeystoreType() {
    if (BouncyCastleKeyStore.Exists(context)) {
      keystoreType = BaseKeyStore.KeystoreType.BKS;
    } else if (AndroidKeyStore.Exists(context)) {
      keystoreType = BaseKeyStore.KeystoreType.ANDROID;
    } else {
      keystoreType = BaseKeyStore.KeystoreType.NONE;
    }
  }

  public void GenerateAndStoreKey() {
    BaseKeyStore.Log("TMG", "== Generate And Store Key - Start");
    SetKeystoreType();
    if (keystoreType == BaseKeyStore.KeystoreType.BKS) {
      if (!BouncyCastleKeyStore.Generate(context)) {
        edit().clear().commit();
        throw new RuntimeException("Failed to Generate BKS Keystore file");
      }
    } else if (!AndroidKeyStore.Generate(context)) {
      edit().clear().commit();
      BouncyCastleKeyStore.Generate(context);
    }
    SetKeystoreType();
    BaseKeyStore.Log("TMG", "== Generate And Store Key - End");
  }

  protected String encrypt(String value) {
    switch (keystoreType) {
      case ANDROID:
        return AndroidKeyStore.Encrypt(value);
      case BKS:
        return BouncyCastleKeyStore.Encrypt(value);
      default:
        throw new RuntimeException("Tried to encrypt with no keystore - Failing");
    }
  }

  protected String decrypt(String value) {
    if (value == null || value.isEmpty()) {
      return value;
    }
    switch (keystoreType) {
      case ANDROID:
        return AndroidKeyStore.Decrypt(value);
      case BKS:
        return BouncyCastleKeyStore.Decrypt(value);
      default:
        throw new RuntimeException("Tried to decrypt with no keystore - Failing");
    }
  }

  private String read(String key) {
    String raw = delegate.getString(key, null);
    String value = decrypt(raw);
    if (value == null) {
      delegate.edit().remove(key).commit();
    }
    return raw != null ? value : null;
  }

  @Override
  public boolean contains(String key) {
    return delegate.contains(key);
  }

  @Override
  public Map<String, ?> getAll() {
    throw new UnsupportedOperationException();
  }

  @Override
  public boolean getBoolean(String key, boolean defValue) {
    String value = read(key);
    return value != null ? Boolean.parseBoolean(value) : defValue;
  }

  @Override
  public float getFloat(String key, float defValue) {
    String value = read(key);
    return value != null ? Float.parseFloat(value) : defValue;
  }

  @Override
  public int getInt(String key, int defValue) {
    String value = read(key);
    return value != null ? Integer.parseInt(value) : defValue;
  }

  @Override
  public long getLong(String key, long defValue) {
    String value = read(key);
    return value != null ? Long.parseLong(value) : defValue;
  }

  @Override
  public String getString(String key, String defValue) {
    String value = read(key);
    return value != null ? value : defValue;
  }

  @Override
  public Set<String> getStringSet(String key, Set<String> defValues) {
    Set<String> raw = delegate.getStringSet(key, null);
    if (raw == null) {
      return defValues;
    }
    Set<String> result = new HashSet<>();
    for (String item : raw) {
      String value = decrypt(item);
      if (value == null) {
        delegate.edit().remove(key).commit();
        return defValues;
      }
      result.add(value);
    }
    return result.isEmpty() ? defValues : result;
  }

  @Override
  public Editor edit() {
    return new Editor();
  }

  @Override
  public void registerOnSharedPreferenceChangeListener(OnSharedPreferenceChangeListener listener) {
    delegate.registerOnSharedPreferenceChangeListener(listener);
  }

  @Override
  public void unregisterOnSharedPreferenceChangeListener(OnSharedPreferenceChangeListener listener) {
    delegate.unregisterOnSharedPreferenceChangeListener(listener);
  }

  public class Editor implements SharedPreferences.Editor {
    protected final SharedPreferences.Editor delegate;

    public Editor() {
      delegate = ObscuredSharedPreferences.this.delegate.edit();
    }

    @Override
    public Editor putBoolean(String key, boolean value) {
      delegate.putString(key, encrypt(Boolean.toString(value)));
      return this;
    }

    @Override
    public Editor putFloat(String key, float value) {
      delegate.putString(key, encrypt(Float.toString(value)));
      return this;
    }

    @Override
    public Editor putInt(String key, int value) {
      delegate.putString(key, encrypt(Integer.toString(value)));
      return this;
    }

    @Override
    public Editor putLong(String key, long value) {
      delegate.putString(key, encrypt(Long.toString(value)));
      return this;
    }

    @Override
    public Editor putString(String key, String value) {
      delegate.putString(key, encrypt(value));
      return this;
    }

    @Override
    public Editor putStringSet(String key, Set<String> values) {
      Set<String> encrypted = new HashSet<>();
      for (String value : values) {
        encrypted.add(encrypt(value));
      }
      delegate.putStringSet(key, encrypted);
      return this;
    }

    @Override
    public Editor remove(String key) {
      delegate.remove(key);
      return this;
    }

    @Override
    public Editor clear() {
      delegate.clear();
      return this;
    }

    @Override
    public boolean commit() {
      return delegate.commit();
    }

    @Override
    public void apply() {
      delegate.apply();
    }
  }
}
