# macOS Native Plugins (Apple Silicon + Intel)

This folder adds **KeyChainOSX** and **MemoryMonitorOSX** native plugins for Unity.

## What you get

- **libKeyChainOSX.dylib**
  - Provides `_cryptProtectData`, `_cryptUnprotectData`, and `_keyChainFree` used by `KeyChainWindowsManager`.
  - Also provides `_setAppData` and `_getAppData` for projects that still use `KeyChainIOSManager` on macOS.
  - Stores a per-user 32-byte master key in macOS Keychain (Service: `OpenCPI.KeyChainOSX`, Account: `MasterKey_v1`).

- **libMemoryMonitorOSX.dylib**
  - Provides `_GetHeapSize`, `_GetFreeBytes`, `_GetTotalBytes`, `_GetBatteryPercent`, `_GetProcessUsedBytes`.

## Build

From this folder:

```bash
./build.sh
```

## Install into Unity

Copy the built dylibs into your project:

- `Assets/Plugins/macOS/libKeyChainOSX.dylib`
- `Assets/Plugins/macOS/libMemoryMonitorOSX.dylib`

After that, Unity should resolve:

- `[DllImport("KeyChainOSX")]` via `libKeyChainOSX.dylib`
- `[DllImport("MemoryMonitorOSX")]` via `libMemoryMonitorOSX.dylib`
