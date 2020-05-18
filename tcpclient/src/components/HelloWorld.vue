<template>
  <div class="hello">
    <h2>WebSocket Test</h2>
    <textarea cols=60 rows=6></textarea>
    <button @click="onClickButton">send</button>
    <div id=output></div>
  </div>
</template>

<script>


 // http://www.websocket.org/echo.html

    var button = document.querySelector("button"),
        output = document.querySelector("#output"),
        textarea = document.querySelector("textarea"),
        // wsUri = "ws://echo.websocket.org/",
        wsUri = "ws:10.0.0.213/",
        websocket = new WebSocket(wsUri);
 

    websocket.onopen = function (e) {
        writeToScreen("CONNECTED");
        doSend("WebSocket rocks");
    };

    websocket.onclose = function (e) {
        writeToScreen("DISCONNECTED");
    };

    websocket.onmessage = function (e) {
        writeToScreen("<span>RESPONSE: " + e.data + "</span>");
    };

    websocket.onerror = function (e) {
        writeToScreen("<span class=error>ERROR:</span> " + e.data);
    };

 
export default {
  name: 'HelloWorld',
  methods: {
    onClickButton() {
        var text = textarea.value;

        text && doSend(text);
        textarea.value = "";
        textarea.focus();
    }, 
    doSend(message) {
        writeToScreen("SENT: " + message);
        websocket.send(message);
    }, 
    writeToScreen(message) {
        output.insertAdjacentHTML("afterbegin", "<p>" + message + "</p>");
    }, 

  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
 
    textarea { vertical-align: bottom; }
    #output { overflow: auto; }
    #output > p { overflow-wrap: break-word; }
    #output span { color: blue; }
    #output span.error { color: red; }
 
</style>
