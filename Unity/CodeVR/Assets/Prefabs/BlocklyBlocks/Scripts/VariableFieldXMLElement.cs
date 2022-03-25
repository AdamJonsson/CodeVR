using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class VariableFieldXMLElement : CustomXMLElement
{
    [SerializeField] private DropdownInput _input;

    public override XmlElement GetXmlElement(XmlDocument document)
    {
        if (this._input.Value == null) return null;
        var xmlField = document.CreateElement("field");
        xmlField.SetAttribute("name", "VAR");
        xmlField.SetAttribute("id", _input.Value);
        xmlField.InnerText = _input.SelectedOption.Text;
        return xmlField;
    }
}