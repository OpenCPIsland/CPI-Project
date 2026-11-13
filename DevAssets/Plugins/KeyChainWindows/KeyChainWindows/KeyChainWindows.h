#pragma once

// The following ifdef block is the standard way of creating macros which make exporting
// from a DLL simpler. All files within this DLL are compiled with the KEYCHAINWINDOWS_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see
// KEYCHAINWINDOWS_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef KEYCHAINWINDOWS_EXPORTS
#define KEYCHAINWINDOWS_API __declspec(dllexport)
#else
#define KEYCHAINWINDOWS_API __declspec(dllimport)
#endif

#include "pch.h"

#ifdef __cplusplus
extern "C" {
#endif

// _cryptProtectData: input string -> output buffer + size
// Returns 1 on success, 0 on failure
KEYCHAINWINDOWS_API int _cryptProtectData(const char* dataIn, int* sizeOut, char** dataOut);

// _cryptUnprotectData: input buffer + size -> output string
// Returns 1 on success, 0 on failure
KEYCHAINWINDOWS_API int _cryptUnprotectData(const unsigned char* dataIn, int size, char** dataOut);

// _keyChainFree: free memory allocated by _cryptProtectData / _cryptUnprotectData
KEYCHAINWINDOWS_API void _keyChainFree(void* ptr);

#ifdef __cplusplus
}
#endif
