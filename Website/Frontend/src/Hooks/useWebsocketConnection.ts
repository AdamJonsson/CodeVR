import { useState, useEffect } from "react";
import { WebSocketConnectionStatus } from "./WebSocketConnectionStatus";

function useWebsocketConnection<T>(
  channel: string
): [T | null, WebSocketConnectionStatus] {
  console.log("Use Unity blockly xml");

  const [websocketData, setWebsocketData] = useState<T | null>(null);
  const [connectionStatus, setConnectionStatus] = useState(
    WebSocketConnectionStatus.Unknown
  );

  useEffect(() => {
    var blocklyWebSocket = new WebSocket(
      `ws://${process.env.REACT_APP_ADRESS}:8999`
    );

    blocklyWebSocket.onopen = () => {
      console.log("Socket open!");
      setConnectionStatus(WebSocketConnectionStatus.Success);
    };

    blocklyWebSocket.onerror = () => {
      setConnectionStatus(WebSocketConnectionStatus.Error);
    };

    blocklyWebSocket.onclose = () => {
      setConnectionStatus(WebSocketConnectionStatus.LostConnection);
    };

    blocklyWebSocket.onmessage = (message) => {
      var parsedData;
      try {
        parsedData = JSON.parse(message.data);
      } catch (error) {
        console.log("Could not parse message from websocket!");
        return;
      }

      if (parsedData.channel === channel) {
        setWebsocketData(parsedData.data);
      }
    };

    return function cleanup() {
      blocklyWebSocket.close();
    };
  }, [channel]);

  return [websocketData, connectionStatus];
}

export default useWebsocketConnection;
