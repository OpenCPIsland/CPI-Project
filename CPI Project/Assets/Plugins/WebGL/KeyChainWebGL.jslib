mergeInto(LibraryManager.library, {
  KeyChainWebGL_SetString: function (keyPtr, valuePtr) {
    var key = UTF8ToString(keyPtr);
    var value = UTF8ToString(valuePtr);

    try {
      if (typeof localStorage !== 'undefined' && localStorage) {
        localStorage.setItem(key, value);
      }
    } catch (e) {
    }
  },

  KeyChainWebGL_GetStringPtr: function (keyPtr) {
    var key = UTF8ToString(keyPtr);
    var value = "";

    try {
      if (typeof localStorage !== 'undefined' && localStorage) {
        var v = localStorage.getItem(key);
        value = (v === null) ? "" : v;
      }
    } catch (e) {
      value = "";
    }

    var size = lengthBytesUTF8(value) + 1;
    var buffer = _malloc(size);
    stringToUTF8(value, buffer, size);
    return buffer;
  },

  KeyChainWebGL_RemoveString: function (keyPtr) {
    var key = UTF8ToString(keyPtr);

    try {
      if (typeof localStorage !== 'undefined' && localStorage) {
        localStorage.removeItem(key);
      }
    } catch (e) {
    }
  },

  KeyChainWebGL_HasKey: function (keyPtr) {
    var key = UTF8ToString(keyPtr);

    try {
      if (typeof localStorage !== 'undefined' && localStorage) {
        return (localStorage.getItem(key) !== null) ? 1 : 0;
      }
    } catch (e) {
    }

    return 0;
  },

  KeyChainWebGL_Free: function (ptr) {
    try {
      _free(ptr);
    } catch (e) {
    }
  }
});
