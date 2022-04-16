
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class FunctionCallBlockXMLElement : CustomXMLElement
{
    [SerializeField] private FunctionCallBlock _functionCallBlock;

    public override XmlElement GetXmlElement(XmlDocument document)
    {
        var selectedFunction = this._functionCallBlock.SelectedFunction;
        if (selectedFunction == null) return null;

        var xmlField = document.CreateElement("mutation");
        xmlField.SetAttribute("name", selectedFunction.Name);
        foreach (var parameter in selectedFunction.GetCurrentParameters())
        {
            var argXmlField = document.CreateElement("arg");
            argXmlField.SetAttribute("name", parameter.DropdownInput.SelectedOption.Text);
            xmlField.AppendChild(argXmlField);
        }
        
        return xmlField;
    }
}