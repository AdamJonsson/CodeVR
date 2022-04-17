using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ControlsIfXMLElement : CustomXMLElement
{
    [SerializeField] private CodeBlockConnector _nextConnector;

    public override XmlElement GetXmlElement(XmlDocument document)
    {
        var numberOfElseBlocks = 0;
        var numberOfElseIfBlocks = 0;
        if (this._nextConnector.IsConnected)
        {
            if (this.HasElseBlock(this._nextConnector))
                numberOfElseBlocks = 1;
            numberOfElseIfBlocks = GetNumberOfElseIf(this._nextConnector);
        }
        
        var mutationXmlElement = document.CreateElement("mutation");
        mutationXmlElement.SetAttribute("else", numberOfElseBlocks.ToString());
        mutationXmlElement.SetAttribute("elseif", numberOfElseIfBlocks.ToString());
        return mutationXmlElement;
    }

    private int GetNumberOfElseIf(CodeBlockConnector nextConnector, int number = 0)
    {
        if (!nextConnector.IsConnected) return number;
        if (nextConnector.BlockConnectedTo.BlocklyTypeString != "controls_else_if") return number;
        return GetNumberOfElseIf(nextConnector.BlockConnectedTo.NextConnector, number + 1);
    }

    private bool HasElseBlock(CodeBlockConnector nextConnector)
    {
        if (!nextConnector.IsConnected) return false;
        if (nextConnector.BlockConnectedTo.BlocklyTypeString == "controls_else") return true;
        if (nextConnector.BlockConnectedTo.BlocklyTypeString == "controls_else_if") return this.HasElseBlock(nextConnector.BlockConnectedTo.NextConnector);
        return false;
    }
}
