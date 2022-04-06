using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SetVariableXMLElement : CustomXMLElement
{

    [SerializeField] private CodeBlockConnector _connector;

    public override XmlElement GetXmlElement(XmlDocument document)
    {
        if (!this._connector.IsConnected) return null;
        VariableDropdownHandler variableData = this._connector.BlockConnectedTo.GetComponent<VariableDropdownHandler>();
        var xmlField = document.CreateElement("field");
        xmlField.SetAttribute("name", "VAR");
        xmlField.SetAttribute("id", variableData.DropdownInput.SelectedOption.Value);
        xmlField.InnerText = variableData.DropdownInput.SelectedOption.Text; 
        return xmlField;
    }

    protected override void Start()
    {
        base.Start();
    }

}
