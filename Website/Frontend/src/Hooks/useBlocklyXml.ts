import { useState, useEffect } from 'react';
import config from '../config.json';

export enum WebSocketConnectionStatus {
  Unknown,
  Error,
  LostConnection,
  Success,
}

function useUnityBlocklyXml(): [string, WebSocketConnectionStatus] {
  console.log("Use Unity blockly xml");

  const [blocklyXml, setBlocklyXml] = useState(`
    <xml xmlns="https://developers.google.com/blockly/xml">
        <block type="text" id="(2|s0Y,[#@Vo|rVdztx;" x="-100" y="-100">
            <field name="TEXT">Generated code will appear here</field>
        </block>
    </xml>
  `);

  const [connectionStatus, setConnectionStatus] = useState(WebSocketConnectionStatus.Unknown);

  useEffect(() => {

    var blocklyWebSocket = new WebSocket(`ws://${config.serverIP}:8999`);
    
    blocklyWebSocket.onopen = () => {
      console.log("Socket open!");
      setConnectionStatus(WebSocketConnectionStatus.Success);
    }

    blocklyWebSocket.onerror = () => {
      setConnectionStatus(WebSocketConnectionStatus.Error);
    }
    
    blocklyWebSocket.onclose = () => {
      setConnectionStatus(WebSocketConnectionStatus.LostConnection);
    }

    blocklyWebSocket.onmessage = (message) => {
      var parsedData; 
      try {
        parsedData = JSON.parse(message.data);
      } catch (error) {
        console.log("Could not parse message from websocket!");
        return;
      }
      setBlocklyXml(parsedData.blocklyXML);
    }
  }, []);

  return [blocklyXml, connectionStatus];
}

export default useUnityBlocklyXml;