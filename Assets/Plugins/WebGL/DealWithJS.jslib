mergeInto(LibraryManager.library, {

  SendAuthData: function (str) {
    var convertedText = Pointer_stringify(str);
    localStorage.setItem('student', convertedText);
  },

  ReceiveAuthData: function () {
      var textToPass = localStorage.getItem('student')
      if (!textToPass) {
        window.location.href = "/idnomadas/index.html"
        return
      }
      var bufferSize = lengthBytesUTF8(textToPass) + 1;
      var buffer = _malloc(bufferSize);

      stringToUTF8(textToPass, buffer, bufferSize);

      return buffer;
  },

  OpenURL: function(url) {
        var convertedText = Pointer_stringify(url);
        window.open(convertedText,"_self")
  },

  FileUpload: function(str) {
    var convertedText = Pointer_stringify(str);
    uploadFiles(convertedText);
  }

});