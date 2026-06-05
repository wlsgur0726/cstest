mergeInto(LibraryManager.library, {
  WebSocketTest_Connect: function (urlPtr) {
    var url = UTF8ToString(urlPtr);

    console.log("[WebSocketTest] connect:", url);

    try {
      var socket = new WebSocket(url);

      if (!Module.WebSocketTest_sockets) {
        Module.WebSocketTest_sockets = [];
      }

      var socketId = Module.WebSocketTest_sockets.length;
      Module.WebSocketTest_sockets.push(socket);

      socket.onopen = function () {
        console.log("[WebSocketTest] open:", socketId, url);

        try {
          socket.send("hello");
          console.log("[WebSocketTest] send:", socketId, "hello");
        } catch (e) {
          console.error("[WebSocketTest] send failed:", socketId, e);
        }
      };

      socket.onmessage = function (event) {
        console.log("[WebSocketTest] recv:", socketId, event.data);
      };

      socket.onclose = function (event) {
        console.log(
          "[WebSocketTest] close:",
          socketId,
          "code=", event.code,
          "reason=", event.reason,
          "wasClean=", event.wasClean);
      };

      socket.onerror = function (event) {
        console.error("[WebSocketTest] error:", socketId, event);
      };
    } catch (e) {
      console.error("[WebSocketTest] connect failed:", url, e);
    }
  }
});