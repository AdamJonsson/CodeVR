using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsIf : MonoBehaviour
{

    [SerializeField] private CodeBlock _codeBlock;


    // Start is called before the first frame update
    void Start()
    {
        this._codeBlock.OnConnectionsChangedToCluster += SetConnectorXmlDataForElseIfBlocks;
    }

    private void SetConnectorXmlDataForElseIfBlocks()
    {
        if (!this._codeBlock.NextConnector.IsConnected) return;
        this.SetConnectorXmlDataForElseIfBlock(this._codeBlock.NextConnector.BlockConnectedTo, 0);
    }

    private void SetConnectorXmlDataForElseIfBlock(CodeBlock block, int index)
    {
        if (block.BlocklyTypeString != "controls_else_if") return;
        var controlsElseIf = block.GetComponent<ControlsElseIf>();
        controlsElseIf.SetDoAndIfXmlData(index);
        if (block.NextConnector.IsConnected)
            this.SetConnectorXmlDataForElseIfBlock(block.NextConnector.BlockConnectedTo, index + 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
