package com.disney.mobilenetwork.plugins;

import java.security.KeyStore;
import java.security.Provider;
import java.security.SecureRandom;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Random;
import org.bouncycastle.jce.provider.BouncyCastleProvider;

public class BaseKeyStore {
  public enum KeystoreType {
    NONE,
    BKS,
    ANDROID
  }

  private static final char[] DATA = "abcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()-_=+`~ABCDEFGHIJKLMNOPQRSTUVWXYZ,<.>/?[{]} |".toCharArray();
  private static final SecureRandom RANDOM = new SecureRandom();
  private static Provider bcProvider;

  protected static final String UTF8 = "utf-8";
  protected static KeyStore keystore;

  protected static void Log(String tag, String message) {
  }

  protected static synchronized Provider bouncyCastle() {
    if (bcProvider == null) {
      bcProvider = new BouncyCastleProvider();
    }
    return bcProvider;
  }

  protected static byte[] generateKey(int length) {
    byte[] bytes = new byte[length];
    RANDOM.nextBytes(bytes);
    return bytes;
  }

  protected static String generateWord(long seed) {
    List<Character> chars = new ArrayList<>(DATA.length);
    for (char c : DATA) {
      chars.add(c);
    }
    Random random = new Random(seed);
    for (int i = 0; i < 5; i++) {
      Collections.shuffle(chars, random);
    }
    int length = random.nextInt(31) + 30;
    StringBuilder builder = new StringBuilder(length);
    for (int i = 0; i < length; i++) {
      builder.append(chars.get(random.nextInt(chars.size())));
    }
    return builder.toString();
  }
}
