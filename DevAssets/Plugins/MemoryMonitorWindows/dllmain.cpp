// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <windows.h>
#include <psapi.h>

// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the MEMORYMONITORWINDOWS_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// MEMORYMONITORWINDOWS_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef MEMORYMONITORWINDOWS_EXPORTS
#define MEMORYMONITORWINDOWS_API __declspec(dllexport)
#else
#define MEMORYMONITORWINDOWS_API __declspec(dllimport)
#endif

extern "C" {

// Returns approximate memory usage (Working Set) of the current process
MEMORYMONITORWINDOWS_API SIZE_T _getProcessUsedBytes() {
    HANDLE hProcess = GetCurrentProcess();
    PROCESS_MEMORY_COUNTERS pmc;
    if (K32GetProcessMemoryInfo(hProcess, &pmc, sizeof(pmc))) {
        return pmc.WorkingSetSize;
    }
    return 0;
}

} // extern "C"

// Standard DLL entry point
BOOL APIENTRY DllMain(HMODULE hModule,
                      DWORD  ul_reason_for_call,
                      LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
