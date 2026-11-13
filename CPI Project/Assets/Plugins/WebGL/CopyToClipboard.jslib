mergeInto(LibraryManager.library, {
    CopyToClipboard: function (strPtr) {
        var text = UTF8ToString(strPtr);

        navigator.clipboard.writeText(text).catch(function(err) {
            console.log(err);
        });
    },
    PasteFromClipboard: function (gameObjectPtr, methodPtr) {
        var gameObject = UTF8ToString(gameObjectPtr);
        var method = UTF8ToString(methodPtr);

        navigator.clipboard.readText()
            .then(function(text) {
                SendMessage(gameObject, method, text);
            })
            .catch(function(err) {
                console.log(err);
            });
    }
});