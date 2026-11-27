#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/stat.h>
#include <errno.h>
#include <openssl/evp.h>
#include <openssl/rand.h>

#ifdef __cplusplus
extern "C" {
#endif

#define KEY_FILE "/.local/share/KeyChainLinux/keychain.key"
#define KEY_FILE_XDG "/KeyChainLinux/keychain.key"
#define KEY_SIZE 32

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
static int get_key_path(char* buf, size_t buf_size) {
    const char* home = getenv("HOME");
    if (!home || home[0] == '\0') home = ".";
    int n = snprintf(buf, buf_size, "%s%s", home, KEY_FILE);
    if (n < 0 || (size_t)n >= buf_size) return 0;
    if (access(buf, F_OK) == 0) return 1;

    const char* xdg = getenv("XDG_DATA_HOME");
    if (xdg && xdg[0] == '/') {
        n = snprintf(buf, buf_size, "%s%s", xdg, KEY_FILE_XDG);
        if (n < 0 || (size_t)n >= buf_size) return 0;
    }
    return 1;
}

static int read_key_file(const char* path, unsigned char* key) {
    int fd = open(path, O_RDONLY | O_CLOEXEC);
    if (fd < 0) return errno == ENOENT ? 0 : -1;

    size_t total = 0;
    while (total < KEY_SIZE) {
        ssize_t r = read(fd, key + total, KEY_SIZE - total);
        if (r < 0) {
            if (errno == EINTR) continue;
            close(fd);
            return -1;
        }
        if (r == 0) break;
        total += (size_t)r;
    }

    unsigned char extra;
    ssize_t more = read(fd, &extra, 1);
    close(fd);
    return (total == KEY_SIZE && more == 0) ? 1 : -1;
}

static int write_all(int fd, const unsigned char* data, size_t len) {
    size_t total = 0;
    while (total < len) {
        ssize_t w = write(fd, data + total, len - total);
        if (w < 0) {
            if (errno == EINTR) continue;
            return 0;
        }
        total += (size_t)w;
    }
    return 1;
}

// Load or generate per-user AES-256 key
static int load_or_generate_key(unsigned char* key) {
    char path[512];
    if (!get_key_path(path, sizeof(path))) return 0;

    // Ensure directory exists
    char dir[512];
    strcpy(dir, path);
    char* last_slash = strrchr(dir, '/');
    if (last_slash) {
        *last_slash = 0;
        ensure_directory_exists_recursive(dir);
    }

    int status = read_key_file(path, key);
    if (status == 1) return 1;
    if (status < 0) return 0;

    // Generate new key
    if (RAND_bytes(key, KEY_SIZE) != 1) return 0;

    char tmp[600];
    int n = snprintf(tmp, sizeof(tmp), "%s.XXXXXX", path);
    if (n < 0 || (size_t)n >= sizeof(tmp)) return 0;

    int fd = mkstemp(tmp);
    if (fd < 0) return 0;
    fchmod(fd, 0600);

    int ok = write_all(fd, key, KEY_SIZE) && fsync(fd) == 0;
    if (close(fd) != 0) ok = 0;
    if (!ok) {
        unlink(tmp);
        return 0;
    }

    int linked = link(tmp, path);
    int link_errno = errno;
    unlink(tmp);

    if (linked == 0) return 1;

    if (link_errno == EEXIST) return read_key_file(path, key) == 1;
    return 0;
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
    OPENSSL_cleanse(key, sizeof(key));
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
    OPENSSL_cleanse(key, sizeof(key));
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
