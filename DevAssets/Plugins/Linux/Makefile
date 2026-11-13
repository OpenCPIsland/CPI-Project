CC = gcc
CFLAGS = -fPIC -Wall -O2
LDFLAGS = -shared
BUILD_DIR = build
SRC_DIR = src

# Source files
SOURCES = $(SRC_DIR)/MemoryMonitorLinux.c $(SRC_DIR)/KeyChainLinux.c

# Output .so files
TARGETS = $(BUILD_DIR)/libMemoryMonitorLinux.so $(BUILD_DIR)/libKeyChainLinux.so

all: $(BUILD_DIR) $(TARGETS)

$(BUILD_DIR):
	mkdir -p $(BUILD_DIR)

$(BUILD_DIR)/libMemoryMonitorLinux.so: $(SRC_DIR)/MemoryMonitorLinux.c | $(BUILD_DIR)
	$(CC) $(CFLAGS) $(LDFLAGS) -o $@ $<

$(BUILD_DIR)/libKeyChainLinux.so: $(SRC_DIR)/KeyChainLinux.c | $(BUILD_DIR)
	$(CC) $(CFLAGS) $(LDFLAGS) -o $@ $< -lcrypto -lssl

clean:
	rm -rf $(BUILD_DIR)
