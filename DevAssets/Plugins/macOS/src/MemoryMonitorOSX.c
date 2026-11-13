#include <CoreFoundation/CoreFoundation.h>
#include <mach/mach.h>
#include <mach/mach_host.h>
#include <sys/sysctl.h>
#include <stdint.h>

#include <IOKit/ps/IOPowerSources.h>
#include <IOKit/ps/IOPSKeys.h>

#ifdef __cplusplus
extern "C" {
#endif

static uint64_t get_total_physical_bytes(void)
{
    uint64_t mem = 0;
    size_t len = sizeof(mem);
    if (sysctlbyname("hw.memsize", &mem, &len, NULL, 0) != 0)
    {
        return 0;
    }
    return mem;
}

static uint64_t get_free_bytes(void)
{
    mach_port_t host_port = mach_host_self();
    vm_size_t page_size = 0;
    if (host_page_size(host_port, &page_size) != KERN_SUCCESS)
    {
        return 0;
    }

    vm_statistics64_data_t vm_stat;
    mach_msg_type_number_t count = HOST_VM_INFO64_COUNT;
    if (host_statistics64(host_port, HOST_VM_INFO64, (host_info64_t)&vm_stat, &count) != KERN_SUCCESS)
    {
        return 0;
    }

    return (uint64_t)vm_stat.free_count * (uint64_t)page_size;
}

static uint64_t get_process_used_bytes(void)
{
    task_vm_info_data_t info;
    mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
    kern_return_t kr = task_info(mach_task_self(), TASK_VM_INFO, (task_info_t)&info, &count);
    if (kr == KERN_SUCCESS)
    {
        return (uint64_t)info.phys_footprint;
    }

    mach_task_basic_info_data_t basic;
    count = MACH_TASK_BASIC_INFO_COUNT;
    kr = task_info(mach_task_self(), MACH_TASK_BASIC_INFO, (task_info_t)&basic, &count);
    if (kr == KERN_SUCCESS)
    {
        return (uint64_t)basic.resident_size;
    }

    return 0;
}

static float get_battery_percent(void)
{
    CFTypeRef blob = IOPSCopyPowerSourcesInfo();
    if (!blob)
    {
        return 1.0f;
    }

    CFArrayRef sources = IOPSCopyPowerSourcesList(blob);
    if (!sources)
    {
        CFRelease(blob);
        return 1.0f;
    }

    CFIndex count = CFArrayGetCount(sources);
    float percent = 1.0f;

    for (CFIndex i = 0; i < count; i++)
    {
        CFTypeRef ps = CFArrayGetValueAtIndex(sources, i);
        if (!ps)
        {
            continue;
        }

        CFDictionaryRef desc = IOPSGetPowerSourceDescription(blob, ps);
        if (!desc)
        {
            continue;
        }

        CFNumberRef curNum = (CFNumberRef)CFDictionaryGetValue(desc, CFSTR(kIOPSCurrentCapacityKey));
        CFNumberRef maxNum = (CFNumberRef)CFDictionaryGetValue(desc, CFSTR(kIOPSMaxCapacityKey));
        if (!curNum || !maxNum)
        {
            continue;
        }

        int cur = 0;
        int max = 0;
        if (!CFNumberGetValue(curNum, kCFNumberIntType, &cur) || !CFNumberGetValue(maxNum, kCFNumberIntType, &max))
        {
            continue;
        }

        if (max > 0)
        {
            percent = (float)cur / (float)max;
            if (percent < 0.0f) percent = 0.0f;
            if (percent > 1.0f) percent = 1.0f;
            break;
        }
    }

    CFRelease(sources);
    CFRelease(blob);

    return percent;
}

__attribute__((visibility("default")))
uint64_t _GetHeapSize(void)
{
    return get_process_used_bytes();
}

__attribute__((visibility("default")))
uint64_t _GetFreeBytes(void)
{
    return get_free_bytes();
}

__attribute__((visibility("default")))
uint64_t _GetTotalBytes(void)
{
    return get_total_physical_bytes();
}

__attribute__((visibility("default")))
float _GetBatteryPercent(void)
{
    return get_battery_percent();
}

__attribute__((visibility("default")))
uint64_t _GetProcessUsedBytes(void)
{
    return get_process_used_bytes();
}

__attribute__((visibility("default")))
uint64_t _getProcessUsedBytes(void)
{
    return get_process_used_bytes();
}

__attribute__((visibility("default")))
uint64_t get_process_used_bytes(void)
{
    return get_process_used_bytes();
}

#ifdef __cplusplus
}
#endif
