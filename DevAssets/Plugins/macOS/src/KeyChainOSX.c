#include <CoreFoundation/CoreFoundation.h>
#include <Security/Security.h>
#include <CommonCrypto/CommonCryptor.h>
#include <CommonCrypto/CommonHMAC.h>
#include <CommonCrypto/CommonDigest.h>
#include <stdint.h>
#include <stdlib.h>
#include <stdbool.h>
#include <string.h>

#ifdef __cplusplus
extern "C" {
#endif


static const CFStringRef kKCService = CFSTR("OpenCPI.KeyChainOSX");
static const CFStringRef kKCMasterKeyAccount = CFSTR("MasterKey_v1");
static const CFStringRef kKCAppDataAccount = CFSTR("cp.AppData");

static char* g_appDataCache = NULL;

static void kc_free_cache(void)
{
    if (g_appDataCache)
    {
        free(g_appDataCache);
        g_appDataCache = NULL;
    }
}

static CFMutableDictionaryRef kc_make_query(CFStringRef account)
{
    CFMutableDictionaryRef query = CFDictionaryCreateMutable(kCFAllocatorDefault,
                                                            0,
                                                            &kCFTypeDictionaryKeyCallBacks,
                                                            &kCFTypeDictionaryValueCallBacks);
    if (!query) return NULL;

    CFDictionarySetValue(query, kSecClass, kSecClassGenericPassword);
    CFDictionarySetValue(query, kSecAttrService, kKCService);
    CFDictionarySetValue(query, kSecAttrAccount, account);
    return query;
}

static int kc_get_item_data(CFStringRef account, uint8_t** outData, size_t* outLen)
{
    if (!outData || !outLen) return 0;

    *outData = NULL;
    *outLen = 0;

    CFMutableDictionaryRef query = kc_make_query(account);
    if (!query) return 0;

    CFDictionarySetValue(query, kSecReturnData, kCFBooleanTrue);
    CFDictionarySetValue(query, kSecMatchLimit, kSecMatchLimitOne);

    CFTypeRef result = NULL;
    OSStatus status = SecItemCopyMatching(query, &result);

    CFRelease(query);

    if (status != errSecSuccess || !result)
    {
        if (result) CFRelease(result);
        return 0;
    }

    CFDataRef dataRef = (CFDataRef)result;
    CFIndex len = CFDataGetLength(dataRef);
    if (len <= 0)
    {
        CFRelease(result);
        return 0;
    }

    uint8_t* data = (uint8_t*)malloc((size_t)len);
    if (!data)
    {
        CFRelease(result);
        return 0;
    }

    CFDataGetBytes(dataRef, CFRangeMake(0, len), data);

    *outData = data;
    *outLen = (size_t)len;

    CFRelease(result);
    return 1;
}

static int kc_set_item_data(CFStringRef account, const uint8_t* data, size_t len)
{
    if (!data || len == 0) return 0;

    CFMutableDictionaryRef query = kc_make_query(account);
    if (!query) return 0;

    OSStatus status = SecItemCopyMatching(query, NULL);

    if (status == errSecSuccess)
    {
        CFMutableDictionaryRef attrsToUpdate = CFDictionaryCreateMutable(kCFAllocatorDefault,
                                                                        0,
                                                                        &kCFTypeDictionaryKeyCallBacks,
                                                                        &kCFTypeDictionaryValueCallBacks);
        if (!attrsToUpdate)
        {
            CFRelease(query);
            return 0;
        }

        CFDataRef dataRef = CFDataCreate(kCFAllocatorDefault, data, (CFIndex)len);
        if (!dataRef)
        {
            CFRelease(attrsToUpdate);
            CFRelease(query);
            return 0;
        }

        CFDictionarySetValue(attrsToUpdate, kSecValueData, dataRef);
        status = SecItemUpdate(query, attrsToUpdate);

        CFRelease(dataRef);
        CFRelease(attrsToUpdate);
        CFRelease(query);

        return status == errSecSuccess ? 1 : 0;
    }

    CFDataRef dataRef = CFDataCreate(kCFAllocatorDefault, data, (CFIndex)len);
    if (!dataRef)
    {
        CFRelease(query);
        return 0;
    }

    CFDictionarySetValue(query, kSecValueData, dataRef);
    status = SecItemAdd(query, NULL);

    CFRelease(dataRef);
    CFRelease(query);

    return status == errSecSuccess ? 1 : 0;
}

static int kc_load_or_create_master_key(uint8_t keyOut[32])
{
    if (!keyOut) return 0;

    uint8_t* data = NULL;
    size_t len = 0;

    if (kc_get_item_data(kKCMasterKeyAccount, &data, &len))
    {
        if (len == 32)
        {
            memcpy(keyOut, data, 32);
            free(data);
            return 1;
        }

        free(data);
    }

    if (SecRandomCopyBytes(kSecRandomDefault, 32, keyOut) != errSecSuccess)
    {
        memset(keyOut, 0, 32);
        return 0;
    }

    if (!kc_set_item_data(kKCMasterKeyAccount, keyOut, 32))
    {
        memset(keyOut, 0, 32);
        return 0;
    }

    return 1;
}

static void kc_derive_key(const uint8_t master[32], const char* label, uint8_t outKey[32])
{
    CCHmac(kCCHmacAlgSHA256, master, 32, label, strlen(label), outKey);
}

static int kc_consttime_equals(const uint8_t* a, const uint8_t* b, size_t len)
{
    if (!a || !b) return 0;
    uint8_t diff = 0;
    for (size_t i = 0; i < len; i++)
    {
        diff |= (uint8_t)(a[i] ^ b[i]);
    }
    return diff == 0;
}

__attribute__((visibility("default")))
int _cryptProtectData(const char* dataIn, int* sizeOut, char** dataOut)
{
    if (!dataIn || !sizeOut || !dataOut)
    {
        return 0;
    }

    *sizeOut = 0;
    *dataOut = NULL;

    uint8_t master[32];
    if (!kc_load_or_create_master_key(master))
    {
        return 0;
    }

    uint8_t encKey[32];
    uint8_t macKey[32];
    kc_derive_key(master, "enc", encKey);
    kc_derive_key(master, "hmac", macKey);

    size_t inLen = strlen(dataIn) + 1;

    uint8_t iv[16];
    if (SecRandomCopyBytes(kSecRandomDefault, sizeof(iv), iv) != errSecSuccess)
    {
        return 0;
    }

    size_t cipherCap = inLen + kCCBlockSizeAES128;
    uint8_t* cipher = (uint8_t*)malloc(cipherCap);
    if (!cipher)
    {
        return 0;
    }

    size_t cipherLen = 0;
    CCCryptorStatus st = CCCrypt(kCCEncrypt,
                                kCCAlgorithmAES,
                                kCCOptionPKCS7Padding,
                                encKey,
                                32,
                                iv,
                                (const void*)dataIn,
                                inLen,
                                cipher,
                                cipherCap,
                                &cipherLen);

    if (st != kCCSuccess || cipherLen == 0)
    {
        free(cipher);
        return 0;
    }

    size_t outLen = sizeof(iv) + cipherLen + CC_SHA256_DIGEST_LENGTH;
    uint8_t* outBuf = (uint8_t*)malloc(outLen);
    if (!outBuf)
    {
        free(cipher);
        return 0;
    }

    memcpy(outBuf, iv, sizeof(iv));
    memcpy(outBuf + sizeof(iv), cipher, cipherLen);

    uint8_t tag[CC_SHA256_DIGEST_LENGTH];
    CCHmac(kCCHmacAlgSHA256,
           macKey,
           32,
           outBuf,
           sizeof(iv) + cipherLen,
           tag);

    memcpy(outBuf + sizeof(iv) + cipherLen, tag, CC_SHA256_DIGEST_LENGTH);

    free(cipher);

    *sizeOut = (int)outLen;
    *dataOut = (char*)outBuf;

    return 1;
}

__attribute__((visibility("default")))
int _cryptUnprotectData(const unsigned char* dataIn, int dataInLength, char** dataOut)
{
    if (!dataIn || !dataOut || dataInLength <= (int)(16 + CC_SHA256_DIGEST_LENGTH))
    {
        return 0;
    }

    *dataOut = NULL;

    uint8_t master[32];
    if (!kc_load_or_create_master_key(master))
    {
        return 0;
    }

    uint8_t encKey[32];
    uint8_t macKey[32];
    kc_derive_key(master, "enc", encKey);
    kc_derive_key(master, "hmac", macKey);

    const uint8_t* iv = (const uint8_t*)dataIn;
    const uint8_t* tag = (const uint8_t*)dataIn + (dataInLength - (int)CC_SHA256_DIGEST_LENGTH);
    const uint8_t* cipher = (const uint8_t*)dataIn + 16;
    size_t cipherLen = (size_t)dataInLength - 16 - CC_SHA256_DIGEST_LENGTH;

    uint8_t expected[CC_SHA256_DIGEST_LENGTH];
    CCHmac(kCCHmacAlgSHA256,
           macKey,
           32,
           dataIn,
           (size_t)(16 + cipherLen),
           expected);

    if (!kc_consttime_equals(expected, tag, CC_SHA256_DIGEST_LENGTH))
    {
        return 0;
    }

    size_t plainCap = cipherLen + kCCBlockSizeAES128;
    char* plain = (char*)malloc(plainCap + 1);
    if (!plain)
    {
        return 0;
    }

    size_t plainLen = 0;
    CCCryptorStatus st = CCCrypt(kCCDecrypt,
                                kCCAlgorithmAES,
                                kCCOptionPKCS7Padding,
                                encKey,
                                32,
                                iv,
                                cipher,
                                cipherLen,
                                (void*)plain,
                                plainCap,
                                &plainLen);

    if (st != kCCSuccess)
    {
        free(plain);
        return 0;
    }

    if (plainLen == 0)
    {
        plain[0] = '\0';
        plainLen = 1;
    }
    else
    {
        plain[plainLen] = '\0';
    }

    *dataOut = plain;
    return 1;
}

__attribute__((visibility("default")))
void _keyChainFree(void* ptr)
{
    if (ptr)
    {
        free(ptr);
    }
}

__attribute__((visibility("default")))
bool _setAppData(const char* jsonString, int len)
{
    (void)len;

    if (!jsonString)
    {
        return false;
    }

    size_t realLen = strlen(jsonString) + 1;
    return kc_set_item_data(kKCAppDataAccount, (const uint8_t*)jsonString, realLen) ? true : false;
}

__attribute__((visibility("default")))
const char* _getAppData(void)
{
    uint8_t* data = NULL;
    size_t len = 0;

    if (!kc_get_item_data(kKCAppDataAccount, &data, &len) || !data || len == 0)
    {
        kc_free_cache();
        g_appDataCache = (char*)malloc(3);
        if (!g_appDataCache)
        {
            return "{}";
        }
        g_appDataCache[0] = '{';
        g_appDataCache[1] = '}';
        g_appDataCache[2] = '\0';
        return g_appDataCache;
    }

    kc_free_cache();
    g_appDataCache = (char*)malloc(len + 1);
    if (!g_appDataCache)
    {
        free(data);
        return "{}";
    }

    memcpy(g_appDataCache, data, len);
    g_appDataCache[len] = '\0';

    free(data);
    return g_appDataCache;
}

#ifdef __cplusplus
}
#endif
