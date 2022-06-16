mergeInto(LibraryManager.library, {

  Login: function () {
    LoginNear();
  },

  OpenURL: function(url) {
    var convertedText = Pointer_stringify(url);
    window.open(convertedText,"_self")
  },

  GetBalance: function(){
    var returnStr = getBalance()
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  GetAccounID: function(){
    var returnStr = AccountId();
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },
 
});
