package com.disney.mobilenetwork.plugins;

import android.content.Context;
import android.security.keystore.KeyGenParameterSpec;
import android.security.keystore.KeyProperties;
import android.util.Base64;
import java.math.BigInteger;
import java.nio.charset.StandardCharsets;
import java.security.KeyPairGenerator;
import java.security.KeyStore;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.util.Calendar;
import java.util.Enumeration;
import javax.crypto.Cipher;
import javax.security.auth.x500.X500Principal;

public class AndroidKeyStore extends BaseKeyStore {
  private static final int ANDROID_ID = 3311983;
  private static final String PROVIDER = "AndroidKeyStore";
  private static final String TRANSFORMATION = "RSA/ECB/PKCS1Padding";

  private static String alias() {
    return generateWord(ANDROID_ID);
  }

  public static String Decrypt(String value) {
    try {
      byte[] data = value != null ? Base64.decode(value, Base64.DEFAULT) : new byte[0];
      PrivateKey key = (PrivateKey) keystore.getKey(alias(), null);
      Cipher cipher = Cipher.getInstance(TRANSFORMATION);
      cipher.init(Cipher.DECRYPT_MODE, key);
      return new String(cipher.doFinal(data), StandardCharsets.UTF_8);
    } catch (Exception e) {
      return null;
    }
  }

  public static String Encrypt(String value) {
    try {
      byte[] data = value != null ? value.getBytes(StandardCharsets.UTF_8) : new byte[0];
      PublicKey key = keystore.getCertificate(alias()).getPublicKey();
      Cipher cipher = Cipher.getInstance(TRANSFORMATION);
      cipher.init(Cipher.ENCRYPT_MODE, key);
      return new String(Base64.encode(cipher.doFinal(data), Base64.DEFAULT), StandardCharsets.UTF_8);
    } catch (Exception e) {
      throw new RuntimeException("Failed to Encrypt Android: " + e);
    }
  }

  public static boolean Exists(Context context) {
    try {
      KeyStore store = KeyStore.getInstance(PROVIDER);
      store.load(null);
      if (GetAlias(store) != null) {
        Log("TMG", "== Found Android Keychain");
        return true;
      }
    } catch (Exception ignored) {
    }
    return false;
  }

  public static boolean Generate(Context context) {
    Log("TMG", "== Generate And Store Android Key - Start");
    try {
      keystore = KeyStore.getInstance(PROVIDER);
      keystore.load(null);
      if (GetAlias(keystore) == null) {
        Log("TMG", "== Generate And Store Key - No Alias");
        Calendar start = Calendar.getInstance();
        Calendar end = Calendar.getInstance();
        end.add(Calendar.YEAR, 50);
        KeyGenParameterSpec spec = new KeyGenParameterSpec.Builder(alias(), KeyProperties.PURPOSE_ENCRYPT | KeyProperties.PURPOSE_DECRYPT)
            .setCertificateSubject(new X500Principal("CN=Disney, O=Android Authority"))
            .setCertificateSerialNumber(BigInteger.ONE)
            .setCertificateNotBefore(start.getTime())
            .setCertificateNotAfter(end.getTime())
            .setKeySize(2048)
            .setEncryptionPaddings(KeyProperties.ENCRYPTION_PADDING_RSA_PKCS1)
            .setDigests(KeyProperties.DIGEST_NONE, KeyProperties.DIGEST_SHA256)
            .setRandomizedEncryptionRequired(true)
            .build();
        KeyPairGenerator generator = KeyPairGenerator.getInstance(KeyProperties.KEY_ALGORITHM_RSA, PROVIDER);
        generator.initialize(spec);
        generator.generateKeyPair();
      }
      Log("TMG", "== Generate And Store Android Key - Passed");
      return true;
    } catch (Exception e) {
      Log("TMG", "== Generate And Store Android Key - Failed, Trying to Generate BKS Keystore");
      return false;
    }
  }

  private static String GetAlias(KeyStore store) {
    try {
      String target = alias();
      Enumeration<String> aliases = store.aliases();
      while (aliases.hasMoreElements()) {
        String name = aliases.nextElement();
        if (target.equals(name)) {
          return name;
        }
      }
    } catch (Exception ignored) {
    }
    return null;
  }
}
