#!/bin/bash

# Log file
LOG_FILE="build/build.log"

OPENSSL_VERSION="4.1.0"
OPENSSL_SHA256="50c17baf54cb008f6ce42d50deb4b28050a1aad711636cabd4b241c7e90cc2fd"
OPENSSL_PREFIX="$PWD/deps/openssl-$OPENSSL_VERSION"

# Start with a clean build directory
rm -rf build
mkdir -p build
> "$LOG_FILE"

# Function to check command existence
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

pause() {
    if [ -t 0 ]; then
        read -p "Press enter to exit..."
    fi
}

install_packages() {
    if command_exists apt-get; then
        sudo apt-get update
        sudo apt-get install -y build-essential perl curl
    elif command_exists dnf; then
        sudo dnf install -y gcc make perl-core curl
    elif command_exists pacman; then
        sudo pacman -S --needed --noconfirm base-devel perl curl
    elif command_exists zypper; then
        sudo zypper install -y gcc make perl curl
    else
        echo "Unknown package manager. Install gcc, make, perl and curl manually."
        pause
        exit 1
    fi
}

# Install dependencies if missing
if ! command_exists gcc || ! command_exists make || ! command_exists perl || ! command_exists curl; then
    echo "Build tools not found. Installing..."
    install_packages
fi

build_openssl() {
    local src="deps/src"
    local archive="$src/openssl-$OPENSSL_VERSION.tar.gz"
    mkdir -p "$src"
    if ! echo "$OPENSSL_SHA256  $archive" | sha256sum -c --status 2>/dev/null; then
        curl -fL --retry 3 -o "$archive" "https://github.com/openssl/openssl/releases/download/openssl-$OPENSSL_VERSION/openssl-$OPENSSL_VERSION.tar.gz" || return 1
        if ! echo "$OPENSSL_SHA256  $archive" | sha256sum -c --status; then
            echo "OpenSSL $OPENSSL_VERSION download failed the checksum."
            rm -f "$archive"
            return 1
        fi
    fi
    rm -rf "$src/openssl-$OPENSSL_VERSION" "$OPENSSL_PREFIX"
    tar xzf "$archive" -C "$src" || return 1
    (
        cd "$src/openssl-$OPENSSL_VERSION" &&
        ./Configure no-shared no-tests no-docs no-apps no-module -fPIC --prefix="$OPENSSL_PREFIX" --libdir=lib &&
        make -j"$(nproc)" build_libs &&
        make install_dev
    ) || return 1
}

if [ ! -f "$OPENSSL_PREFIX/lib/libcrypto.a" ]; then
    echo "Building OpenSSL $OPENSSL_VERSION..."
    build_openssl 2>&1 | tee -a "$LOG_FILE"
    if [ ${PIPESTATUS[0]} -ne 0 ] || [ ! -f "$OPENSSL_PREFIX/lib/libcrypto.a" ]; then
        echo "OpenSSL build failed. Check $LOG_FILE for details."
        pause
        exit 1
    fi
fi

echo "Building..."
make all OPENSSL_PREFIX="$OPENSSL_PREFIX" 2>&1 | tee -a "$LOG_FILE"
if [ ${PIPESTATUS[0]} -ne 0 ]; then
    echo "Build failed. Check $LOG_FILE for details."
    pause
    exit 1
fi

echo "Build complete! .so files are in build/"
pause

