mergeInto(LibraryManager.library, {

  Login: function () {
    LoginNear();
  },

  OpenURL: function(url) {
    var convertedText = Pointer_stringify(url);
    window.open(convertedText,"_self")
  },
});
