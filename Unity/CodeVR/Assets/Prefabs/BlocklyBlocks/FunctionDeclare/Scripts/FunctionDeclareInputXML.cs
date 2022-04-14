using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class FunctionDeclareInputXML : CustomXMLElement
{
    [SerializeField] private FunctionDeclareBlock _functionDeclareBlock;

    public override XmlElement GetXmlElement(XmlDocument document)
    {
        if (this._functionDeclareBlock == null) return null;
        var xmlMutation = document.CreateElement("mutation");

        foreach (var parameter in this._functionDeclareBlock.GetCurrentParameters())
        {
            var xmlArg = document.CreateElement("arg");
            xmlArg.SetAttribute("name", parameter.DropdownInput.SelectedOption.Text);
            xmlArg.SetAttribute("varid", parameter.DropdownInput.SelectedOption.Value);
            xmlMutation.AppendChild(xmlArg);
        }
        return xmlMutation;
    }
}
