import { useState, useEffect } from "react";
import useWebsocketConnection from "./useWebsocketConnection";
import { WebSocketConnectionStatus } from "./WebSocketConnectionStatus";

function useUnityBlocklyXml(): [string, WebSocketConnectionStatus] {
  console.log("Use Unity blockly xml");

  const [blocklyXml, setBlocklyXml] = useState(`
    <xml xmlns="https://developers.google.com/blockly/xml">
        <block type="text" id="(2|s0Y,[#@Vo|rVdztx;" x="-100" y="-100">
            <field name="TEXT">Generated code will appear here</field>
        </block>
    </xml>
  `);

  const [blocklyXmlFromServer, connectionStatus] =
    useWebsocketConnection<string>("xmlCode");

  useEffect(() => {
    if (blocklyXmlFromServer != null) {
      setBlocklyXml(blocklyXmlFromServer);
    }
  }, [blocklyXmlFromServer]);

  return [blocklyXml, connectionStatus];
}

export default useUnityBlocklyXml;
