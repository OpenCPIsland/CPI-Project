#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <unistd.h>
#include <sys/stat.h>
#include <errno.h>
#include <openssl/evp.h>
#include <openssl/rand.h>

#ifdef __cplusplus
extern "C" {
#endif

#define KEY_FILE "/.local/share/KeyChainLinux/keychain.key"

// Recursively create a directory path (mkdir -p behavior).
static void ensure_directory_exists_recursive(const char* dir_path) {
    if (!dir_path || dir_path[0] == '\0') return;

    char tmp[512];
    size_t len = strlen(dir_path);
    if (len >= sizeof(tmp)) return;

    strcpy(tmp, dir_path);
    if (tmp[len - 1] == '/') tmp[len - 1] = 0;

    for (char* p = tmp + 1; *p; p++) {
        if (*p == '/') {
            *p = 0;
            if (mkdir(tmp, 0700) != 0 && errno != EEXIST) {
                *p = '/';
                return;
            }
            *p = '/';
        }
    }

    mkdir(tmp, 0700);
}

// Helper to get user key path
static void get_key_path(char* buf, size_t buf_size) {
    const char* home = getenv("HOME");
    if (!home) home = ".";
    snprintf(buf, buf_size, "%s%s", home, KEY_FILE);
}

// Load or generate per-user AES-256 key
static int load_or_generate_key(unsigned char* key) {
    char path[512];
    get_key_path(path, sizeof(path));

    // Ensure directory exists
    char dir[512];
    strcpy(dir, path);
    char* last_slash = strrchr(dir, '/');
    if (last_slash) {
        *last_slash = 0;
        ensure_directory_exists_recursive(dir);
    }

    FILE* f = fopen(path, "rb");
    if (f) {
        size_t read = fread(key, 1, 32, f);
        fclose(f);
        if (read == 32) return 1;
    }

    // Generate new key
    if (RAND_bytes(key, 32) != 1) return 0;

    f = fopen(path, "wb");
    if (!f) return 0;
    fwrite(key, 1, 32, f);
    fclose(f);
    chmod(path, 0600);
    return 1;
}

// Encrypt input string (includes terminating null)
__attribute__((visibility("default")))
int _cryptProtectData(const char* dataIn, int* sizeOut, char** dataOut) {
    if (!dataIn || !sizeOut || !dataOut) return 0;

    unsigned char key[32];
    if (!load_or_generate_key(key)) return 0;

    int inLen = (int)strlen(dataIn) + 1; // include terminating null
    unsigned char iv[12];
    if (RAND_bytes(iv, sizeof(iv)) != 1) return 0;

    EVP_CIPHER_CTX* ctx = EVP_CIPHER_CTX_new();
    if (!ctx) return 0;

    if (EVP_EncryptInit_ex(ctx, EVP_aes_256_gcm(), NULL, NULL, NULL) != 1) {
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    if (EVP_EncryptInit_ex(ctx, NULL, NULL, key, iv) != 1) {
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    unsigned char* outBuf = (unsigned char*)malloc(sizeof(iv) + inLen + 16); // IV + ciphertext + tag
    if (!outBuf) {
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    memcpy(outBuf, iv, sizeof(iv)); // prepend IV

    int outLen = 0;
    if (EVP_EncryptUpdate(ctx, outBuf + sizeof(iv), &outLen, (const unsigned char*)dataIn, inLen) != 1) {
        free(outBuf);
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    int finalLen = 0;
    if (EVP_EncryptFinal_ex(ctx, outBuf + sizeof(iv) + outLen, &finalLen) != 1) {
        free(outBuf);
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    unsigned char tag[16];
    if (EVP_CIPHER_CTX_ctrl(ctx, EVP_CTRL_GCM_GET_TAG, sizeof(tag), tag) != 1) {
        free(outBuf);
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    memcpy(outBuf + sizeof(iv) + outLen + finalLen, tag, sizeof(tag)); // append tag

    *sizeOut = sizeof(iv) + outLen + finalLen + sizeof(tag);
    *dataOut = (char*)outBuf;

    EVP_CIPHER_CTX_free(ctx);
    return 1;
}

// Decrypt buffer to original string (with null terminator)
__attribute__((visibility("default")))
int _cryptUnprotectData(const unsigned char* dataIn, int dataInLength, char** dataOut) {
    if (!dataIn || !dataOut || dataInLength < 12 + 16) return 0; // IV + tag minimum

    unsigned char key[32];
    if (!load_or_generate_key(key)) return 0;

    const unsigned char* iv = dataIn;
    const unsigned char* tag = dataIn + dataInLength - 16;
    const unsigned char* ciphertext = dataIn + 12;
    int ciphertextLen = dataInLength - 12 - 16;

    EVP_CIPHER_CTX* ctx = EVP_CIPHER_CTX_new();
    if (!ctx) return 0;

    if (EVP_DecryptInit_ex(ctx, EVP_aes_256_gcm(), NULL, NULL, NULL) != 1) {
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    if (EVP_DecryptInit_ex(ctx, NULL, NULL, key, iv) != 1) {
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    if (EVP_CIPHER_CTX_ctrl(ctx, EVP_CTRL_GCM_SET_TAG, 16, (void*)tag) != 1) {
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    char* outBuf = (char*)malloc(ciphertextLen + 1); // +1 for safety null
    if (!outBuf) {
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    int outLen = 0;
    if (EVP_DecryptUpdate(ctx, (unsigned char*)outBuf, &outLen, ciphertext, ciphertextLen) != 1) {
        free(outBuf);
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    int finalLen = 0;
    if (EVP_DecryptFinal_ex(ctx, (unsigned char*)outBuf + outLen, &finalLen) != 1) {
        free(outBuf);
        EVP_CIPHER_CTX_free(ctx);
        return 0;
    }

    outBuf[outLen + finalLen] = '\0'; // preserve terminating null
    *dataOut = outBuf;

    EVP_CIPHER_CTX_free(ctx);
    return 1;
}

// Free memory allocated by this plugin (must be used instead of Marshal.FreeCoTaskMem on Linux).
__attribute__((visibility("default")))
void _keyChainFree(void* ptr) {
    if (ptr) {
        free(ptr);
    }
}

#ifdef __cplusplus
}
#endif
