#!/usr/bin/env bash
set -euo pipefail

# Builds universal (arm64 + x86_64) dylibs for Unity.
# Output:
#   build/libKeyChainOSX.dylib
#   build/libMemoryMonitorOSX.dylib

ROOT="$(cd "$(dirname "$0")" && pwd)"
SRC="$ROOT/src"
OUT="$ROOT/build"

mkdir -p "$OUT"

SDK_PATH="$(xcrun --sdk macosx --show-sdk-path)"
CLANG="$(xcrun --sdk macosx --find clang)"

COMMON_CFLAGS=(
  -O2
  -fPIC
  -fvisibility=hidden
  -mmacosx-version-min=11.0
  -isysroot "$SDK_PATH"
)

build_one() {
  local name="$1"
  local srcFile="$2"
  shift 2
  local frameworks=("$@")

  echo "Building $name (arm64)..."
  "$CLANG" -dynamiclib "${COMMON_CFLAGS[@]}" -arch arm64 \
    "${frameworks[@]}" \
    -o "$OUT/${name}_arm64.dylib" \
    "$SRC/$srcFile"

  echo "Building $name (x86_64)..."
  "$CLANG" -dynamiclib "${COMMON_CFLAGS[@]}" -arch x86_64 \
    "${frameworks[@]}" \
    -o "$OUT/${name}_x86_64.dylib" \
    "$SRC/$srcFile"

  echo "Creating universal $name..."
  lipo -create \
    "$OUT/${name}_arm64.dylib" \
    "$OUT/${name}_x86_64.dylib" \
    -output "$OUT/$name.dylib"

  rm -f "$OUT/${name}_arm64.dylib" "$OUT/${name}_x86_64.dylib"

  install_name_tool -id "@rpath/$name.dylib" "$OUT/$name.dylib" || true

  echo "Built: $OUT/$name.dylib"
}

build_one "libKeyChainOSX" "KeyChainOSX.c" \
  -framework Security \
  -framework CoreFoundation

build_one "libMemoryMonitorOSX" "MemoryMonitorOSX.c" \
  -framework IOKit \
  -framework CoreFoundation

cat <<MSG

Done.

Copy these into your Unity project:
  Assets/Plugins/macOS/libKeyChainOSX.dylib
  Assets/Plugins/macOS/libMemoryMonitorOSX.dylib

Unity will generate .meta files automatically.

MSG
