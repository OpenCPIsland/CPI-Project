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

// Mirrors Windows ReadProcessMemory API
ssize_t ReadProcessMemory(void *hProcess, void *lpBaseAddress, void *lpBuffer, size_t nSize, size_t *lpNumberOfBytesRead) {
    if (!hProcess || !lpBaseAddress || !lpBuffer) {
        if (lpNumberOfBytesRead) *lpNumberOfBytesRead = 0;
        return 0;
    }

    pid_t pid = (pid_t)(uintptr_t)hProcess; // Use process ID as "handle"
    char mem_path[64];
    int ret = snprintf(mem_path, sizeof(mem_path), "/proc/%d/mem", pid);
    if (ret < 0 || ret >= (int)sizeof(mem_path)) {
        if (lpNumberOfBytesRead) *lpNumberOfBytesRead = 0;
        return 0;
    }

    int fd = open(mem_path, O_RDONLY);
    if (fd == -1) {
        perror("open");
        if (lpNumberOfBytesRead) *lpNumberOfBytesRead = 0;
        return 0;
    }

    ssize_t nread = pread(fd, lpBuffer, nSize, (uintptr_t)lpBaseAddress);
    if (nread == -1) {
        perror("pread");
        nread = 0;
    }

    if (lpNumberOfBytesRead) *lpNumberOfBytesRead = (size_t)nread;
    close(fd);

    return nread > 0;
}

// Exported function for Unity DllImport
__attribute__((visibility("default")))
ssize_t read_process_memory(void *hProcess, void *lpBaseAddress, void *lpBuffer, size_t nSize, size_t *lpNumberOfBytesRead) {
    return ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, nSize, lpNumberOfBytesRead);
}

// Returns approximate memory usage (RSS) in bytes of the current process
__attribute__((visibility("default")))
uint64_t get_process_used_bytes() {
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
