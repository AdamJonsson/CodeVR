import { useState, useEffect } from 'react';
import config from '../config.json';

function useBlocklyXml() {
  const [blocklyXml, setBlocklyXml] = useState(`
    <xml xmlns="https://developers.google.com/blockly/xml">
        <block type="text" id="(2|s0Y,[#@Vo|rVdztx;" x="-100" y="-100">
            <field name="TEXT">Generated code will appear here</field>
        </block>
    </xml>
  `);

  useEffect(() => {

    // TODO: Make a config file for the IP adress.
    var blocklyWebSocket = new WebSocket(`ws://${config.serverIP}:8999`);
    
    blocklyWebSocket.onopen = () => {
      console.log("Socket open!");
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
  });

  return blocklyXml;
}

export default useBlocklyXml;