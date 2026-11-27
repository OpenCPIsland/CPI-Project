// MemoryMonitorLinux.c
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <stdint.h>
#include <errno.h>

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

#ifdef __cplusplus
}
#endif
