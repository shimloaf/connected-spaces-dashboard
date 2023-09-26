var ImageUploaderPlugin = {
  ImageUploaderCaptureClick: function() {
    if (!document.getElementById('ImageUploaderInput')) {
      var fileInput = document.createElement('input');
      fileInput.setAttribute('type', 'file');
      fileInput.setAttribute('id', 'ImageUploaderInput');
      fileInput.style.visibility = 'hidden';
      fileInput.onclick = function (event) {
        this.value = null;
      };
      fileInput.onchange = function (event) {
        SendMessage('Customization Manager', 'FileSelected', URL.createObjectURL(event.target.files[0]));
      }
      document.body.appendChild(fileInput);
    }
    var OpenFileDialog = function() {
      document.getElementById('ImageUploaderInput').click();
      document.getElementById('unity-canvas').removeEventListener('click', OpenFileDialog);
    };
    document.getElementById('unity-canvas').addEventListener('click', OpenFileDialog, false);
  }
};
mergeInto(LibraryManager.library, ImageUploaderPlugin);