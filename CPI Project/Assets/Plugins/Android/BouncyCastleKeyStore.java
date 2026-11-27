package com.disney.mobilenetwork.plugins;

import android.content.Context;
import android.util.Base64;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.nio.charset.StandardCharsets;
import java.security.KeyStore;
import javax.crypto.Cipher;
import javax.crypto.SecretKey;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;

public class BouncyCastleKeyStore extends BaseKeyStore {
  private static final String BKSFilePath = "com.disney.clubpenguinremix_goo.keystore";
  private static final int BKS_ID1 = 9875432;
  private static final int BKS_ID2 = 1321457;
  private static final String KEY_NAME = "COM_DISNEY_MIX_KEY";
  private static final String TRANSFORMATION = "AES/CBC/PKCS5Padding";

  private static KeyStore.PasswordProtection entryPassword() {
    return new KeyStore.PasswordProtection(generateWord(BKS_ID2).toCharArray());
  }

  private static char[] storePassword() {
    return generateWord(BKS_ID1).toCharArray();
  }

  private static SecretKeySpec aesKey() throws Exception {
    KeyStore.SecretKeyEntry entry = (KeyStore.SecretKeyEntry) keystore.getEntry(KEY_NAME, entryPassword());
    byte[] encoded = entry.getSecretKey().getEncoded();
    byte[] key = new byte[16];
    System.arraycopy(encoded, 0, key, 0, Math.min(encoded.length, key.length));
    return new SecretKeySpec(key, "AES");
  }

  public static String Decrypt(String value) {
    try {
      byte[] data = value != null ? Base64.decode(value, Base64.DEFAULT) : new byte[0];
      byte[] iv = new byte[16];
      byte[] body = new byte[data.length - 16];
      System.arraycopy(data, 0, iv, 0, iv.length);
      System.arraycopy(data, 16, body, 0, body.length);
      Cipher cipher = Cipher.getInstance(TRANSFORMATION);
      cipher.init(Cipher.DECRYPT_MODE, aesKey(), new IvParameterSpec(iv));
      return new String(cipher.doFinal(body), StandardCharsets.UTF_8);
    } catch (Exception e) {
      throw new RuntimeException("Failed to Decrypt BKS: " + e);
    }
  }

  public static String Encrypt(String value) {
    try {
      byte[] data = value != null ? value.getBytes(StandardCharsets.UTF_8) : new byte[0];
      byte[] iv = generateKey(16);
      Cipher cipher = Cipher.getInstance(TRANSFORMATION);
      cipher.init(Cipher.ENCRYPT_MODE, aesKey(), new IvParameterSpec(iv));
      byte[] body = cipher.doFinal(data);
      byte[] out = new byte[body.length + 16];
      System.arraycopy(iv, 0, out, 0, 16);
      System.arraycopy(body, 0, out, 16, body.length);
      return new String(Base64.encode(out, Base64.NO_WRAP), StandardCharsets.UTF_8);
    } catch (Exception e) {
      throw new RuntimeException("Failed to Encrypt BKS: " + e);
    }
  }

  public static boolean Exists(Context context) {
    if (context.getFileStreamPath(BKSFilePath).exists()) {
      Log("TMG", "== Found BKS Keychain");
      return true;
    }
    return false;
  }

  public static boolean Generate(Context context) {
    Log("TMG", "== Generate And Store BKS Key - Start");
    File file = context.getFileStreamPath(BKSFilePath);
    try {
      keystore = KeyStore.getInstance("BKS", bouncyCastle());
      try (FileInputStream in = context.openFileInput(BKSFilePath)) {
        keystore.load(in, storePassword());
      } catch (FileNotFoundException missing) {
        keystore.load(null, null);
      }
      if (keystore.getEntry(KEY_NAME, entryPassword()) != null) {
        Log("TMG", "== Generate And Store BKS Key - Entry Already Made - Passed");
        return true;
      }
      SecretKey key = new SecretKeySpec(generateKey(16), "AES");
      keystore.setEntry(KEY_NAME, new KeyStore.SecretKeyEntry(key), entryPassword());
      try (FileOutputStream out = context.openFileOutput(BKSFilePath, Context.MODE_PRIVATE)) {
        keystore.store(out, storePassword());
      }
      Log("TMG", "== Generate And Store BKS Key - Passed");
      return true;
    } catch (Exception e) {
      Log("TMG", "== Generate And Store BKS Key - Failed");
      file.delete();
      return false;
    }
  }
}
