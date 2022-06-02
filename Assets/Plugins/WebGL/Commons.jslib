mergeInto(LibraryManager.library, {

  UploadFiles: function (interaction) {
    var interactionParse = Pointer_stringify(interaction);

    uploadFilesCommons(interaction);
  },

  DownloadImageJs: function(name)
    {
      var convertedText = Pointer_stringify(name);
      DownloadImage(newBytes, convertedText);
    }
});
