// MemoryMonitorLinux.c
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <stdint.h>
#include <errno.h>
#include <string.h>
#include <dirent.h>

#ifdef __cplusplus
extern "C" {
#endif

// Returns approximate memory usage (RSS) in bytes of the current process
__attribute__((visibility("default")))
uint64_t get_process_used_bytes(void) {
    FILE* f = fopen("/proc/self/statm", "r");
    if (!f) return 0;

    long rss_pages = 0;
    if (fscanf(f, "%*s %ld", &rss_pages) != 1) {
        fclose(f);
        return 0;
    }
    fclose(f);

    long page_size = sysconf(_SC_PAGESIZE);
    if (page_size <= 0) page_size = 4096; // fallback

    return (uint64_t)rss_pages * (uint64_t)page_size;
}

static uint64_t meminfo_bytes(const char* field) {
    FILE* f = fopen("/proc/meminfo", "r");
    if (!f) return 0;

    char line[256];
    size_t fieldLen = strlen(field);
    uint64_t result = 0;
    while (fgets(line, sizeof(line), f)) {
        if (strncmp(line, field, fieldLen) == 0 && line[fieldLen] == ':') {
            unsigned long long kb = 0;
            if (sscanf(line + fieldLen + 1, "%llu", &kb) == 1) {
                result = (uint64_t)kb * 1024ULL;
            }
            break;
        }
    }
    fclose(f);
    return result;
}

__attribute__((visibility("default")))
uint64_t get_total_bytes(void) {
    return meminfo_bytes("MemTotal");
}

__attribute__((visibility("default")))
uint64_t get_free_bytes(void) {
    return meminfo_bytes("MemAvailable");
}

static int read_sysfs_line(const char* dir, const char* name, char* buf, size_t size) {
    char path[512];
    int n = snprintf(path, sizeof(path), "/sys/class/power_supply/%s/%s", dir, name);
    if (n < 0 || (size_t)n >= sizeof(path)) return 0;

    FILE* f = fopen(path, "r");
    if (!f) return 0;
    int ok = fgets(buf, (int)size, f) != NULL;
    fclose(f);
    if (ok) buf[strcspn(buf, "\n")] = 0;
    return ok;
}

__attribute__((visibility("default")))
float get_battery_percent(void) {
    DIR* d = opendir("/sys/class/power_supply");
    if (!d) return 1.0f;

    int count = 0;
    int total = 0;
    struct dirent* entry;
    while ((entry = readdir(d)) != NULL) {
        if (entry->d_name[0] == '.') continue;

        char value[64];
        if (!read_sysfs_line(entry->d_name, "type", value, sizeof(value)) || strcmp(value, "Battery") != 0) continue;
        if (read_sysfs_line(entry->d_name, "scope", value, sizeof(value)) && strcmp(value, "Device") == 0) continue;
        if (!read_sysfs_line(entry->d_name, "capacity", value, sizeof(value))) continue;

        int capacity = atoi(value);
        if (capacity < 0 || capacity > 100) continue;
        total += capacity;
        count++;
    }
    closedir(d);

    return count > 0 ? (float)total / (float)(count * 100) : 1.0f;
}

#ifdef __cplusplus
}
#endif
