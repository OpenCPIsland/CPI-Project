#!/bin/bash

# Log file
LOG_FILE="build/build.log"

# Start with a clean build directory
rm -rf build
mkdir -p build

# Function to check command existence
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Install dependencies if missing
if ! command_exists gcc; then
    echo "gcc not found. Installing..."
    sudo apt update
    sudo apt install -y build-essential
fi

if ! command_exists make; then
    echo "make not found. Installing..."
    sudo apt update
    sudo apt install -y make
fi

# Install OpenSSL development package if missing
if ! dpkg -s libssl-dev >/dev/null 2>&1; then
    echo "libssl-dev not found. Installing..."
    sudo apt update
    sudo apt install -y libssl-dev
fi

# Clear previous log
> "$LOG_FILE"

# Compile function
compile_so() {
    local src_file=$1
    local output_file=$2
    local extra_flags=$3  # optional extra flags

    echo "Building $output_file..."
    gcc -fPIC -shared -Wall -O2 "$src_file" $extra_flags -o "$output_file" 2>&1 | tee -a "$LOG_FILE"
    if [ ${PIPESTATUS[0]} -ne 0 ]; then
        echo "Failed to build $output_file. Check $LOG_FILE for details."
        read -p "Press enter to exit..."
        exit 1
    fi
}

# Build MemoryMonitorLinux.so (no special libs)
compile_so "src/MemoryMonitorLinux.c" "build/libMemoryMonitorLinux.so"

# Build KeyChainLinux.so (link OpenSSL)
compile_so "src/KeyChainLinux.c" "build/libKeyChainLinux.so" "-lcrypto -lssl"

echo "Build complete! .so files are in build/"
read -p "Press enter to exit..."

