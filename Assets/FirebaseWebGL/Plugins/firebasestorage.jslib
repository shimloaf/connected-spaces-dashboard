mergeInto(LibraryManager.library, {

    UploadFile: function(path, data, objectName, callback, fallback) {

        var parsedPath = UTF8ToString(path);
        var parsedData = UTF8ToString(data);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {

            firebase.storage().ref(parsedPath).put(base64ToArrayBuffer(parsedData)).then(function(snapshot) {
                unityInstance.Module.SendMessage(parsedObjectName, parsedCallback, "Success: data was posted to " + parsedPath);
            });

        } catch (error) {
            unityInstance.Module.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }

        function base64ToArrayBuffer(base64) {
            var binary_string = window.atob(base64);
            var len = binary_string.length;
            var bytes = new Uint8Array(len);
            for (var i = 0; i < len; i++) {
                bytes[i] = binary_string.charCodeAt(i);
            }
            return bytes.buffer;
        }
    },

    DownloadFile: function(path, objectName, callback, fallback) {

        var parsedPath = UTF8ToString(path);
        var parsedObjectName = UTF8ToString(objectName);
        var parsedCallback = UTF8ToString(callback);
        var parsedFallback = UTF8ToString(fallback);

        try {

            firebase.storage().ref(parsedPath).getDownloadURL().then(function(url) {
                
                var xhr = new XMLHttpRequest();
                xhr.responseType = 'arraybuffer';
                xhr.onload = function(event) {
                    var data = xhr.response;
                    var base64str = base64ArrayBuffer(data);
                  try {
                    unityInstance.SendMessage(parsedObjectName, parsedCallback, base64str);
                  } catch (error) {
                    console.error(error);
                  }
                };
                xhr.open('GET', url);
                xhr.send();
              
              }).catch(function(error) {
                unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
              });

        } catch (error) {
            unityInstance.SendMessage(parsedObjectName, parsedFallback, JSON.stringify(error, Object.getOwnPropertyNames(error)));
        }

        function base64ArrayBuffer(arrayBuffer) {
            var base64    = ''
            var encodings = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'

            var bytes         = new Uint8Array(arrayBuffer)
            var byteLength    = bytes.byteLength
            var byteRemainder = byteLength % 3
            var mainLength    = byteLength - byteRemainder

            var a, b, c, d
            var chunk

            // Main loop deals with bytes in chunks of 3
            for (var i = 0; i < mainLength; i = i + 3) {
                // Combine the three bytes into a single integer
                chunk = (bytes[i] << 16) | (bytes[i + 1] << 8) | bytes[i + 2]

                // Use bitmasks to extract 6-bit segments from the triplet
                a = (chunk & 16515072) >> 18 // 16515072 = (2^6 - 1) << 18
                b = (chunk & 258048)   >> 12 // 258048   = (2^6 - 1) << 12
                c = (chunk & 4032)     >>  6 // 4032     = (2^6 - 1) << 6
                d = chunk & 63               // 63       = 2^6 - 1

                // Convert the raw binary segments to the appropriate ASCII encoding
                base64 += encodings[a] + encodings[b] + encodings[c] + encodings[d]
            }

            // Deal with the remaining bytes and padding
            if (byteRemainder == 1) {
                chunk = bytes[mainLength]

                a = (chunk & 252) >> 2 // 252 = (2^6 - 1) << 2

                // Set the 4 least significant bits to zero
                b = (chunk & 3)   << 4 // 3   = 2^2 - 1

                base64 += encodings[a] + encodings[b] + '=='
            } else if (byteRemainder == 2) {
                chunk = (bytes[mainLength] << 8) | bytes[mainLength + 1]

                a = (chunk & 64512) >> 10 // 64512 = (2^6 - 1) << 10
                b = (chunk & 1008)  >>  4 // 1008  = (2^6 - 1) << 4

                // Set the 2 least significant bits to zero
                c = (chunk & 15)    <<  2 // 15    = 2^4 - 1

                base64 += encodings[a] + encodings[b] + encodings[c] + '='
            }
            
            return base64
        }

        function arrayBufferToBase64( buffer ) {
            var binary = '';
            var bytes = new Uint8Array( buffer );
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode( bytes[ i ] );
            }
            return window.btoa( binary );
        }
    }
});
