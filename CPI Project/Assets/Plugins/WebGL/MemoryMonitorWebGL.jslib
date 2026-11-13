mergeInto(LibraryManager.library, {
  MemoryMonitorWebGL_GetWasmHeapSize: function () {
    try {
      if (typeof wasmMemory !== 'undefined' && wasmMemory && wasmMemory.buffer) {
        return wasmMemory.buffer.byteLength;
      }

      if (typeof Module !== 'undefined' && Module) {
        var mem = Module.wasmMemory || Module['wasmMemory'] || null;
        if (mem && mem.buffer) {
          return mem.buffer.byteLength;
        }
        if (Module.HEAPU8 && Module.HEAPU8.buffer) {
          return Module.HEAPU8.buffer.byteLength;
        }
      }

      if (typeof HEAPU8 !== 'undefined' && HEAPU8 && HEAPU8.buffer) {
        return HEAPU8.buffer.byteLength;
      }
    } catch (e) {
    }

    return 0;
  },

  MemoryMonitorWebGL_GetJsHeapUsed: function () {
    try {
      if (typeof performance !== 'undefined' && performance && performance.memory && typeof performance.memory.usedJSHeapSize === 'number') {
        return performance.memory.usedJSHeapSize;
      }
    } catch (e) {
    }

    return -1;
  },

  MemoryMonitorWebGL_GetJsHeapTotal: function () {
    try {
      if (typeof performance !== 'undefined' && performance && performance.memory && typeof performance.memory.totalJSHeapSize === 'number') {
        return performance.memory.totalJSHeapSize;
      }
    } catch (e) {
    }

    return -1;
  },

  MemoryMonitorWebGL_GetJsHeapLimit: function () {
    try {
      if (typeof performance !== 'undefined' && performance && performance.memory && typeof performance.memory.jsHeapSizeLimit === 'number') {
        return performance.memory.jsHeapSizeLimit;
      }
    } catch (e) {
    }

    return -1;
  }
});
