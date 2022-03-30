using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class FunctionDeclareInputXML : CustomXMLElement
{
    [SerializeField] private CodeBlock _settingBlock;

    public override XmlElement GetXmlElement(XmlDocument document)
    {
        if (this._settingBlock == null) return null;
        var xmlMutation = document.CreateElement("mutation");

        var parameters = this._settingBlock.GetBlockCluster(includeSelf: false).Where((block) => block.BlocklyTypeString == "parameter");
        foreach (var parameter in parameters)
        {
            var variableData = parameter.GetComponent<VariableDropdownHandler>();
            if (variableData == null) continue;
            var xmlArg = document.CreateElement("arg");
            xmlArg.SetAttribute("name", variableData.DropdownInput.SelectedOption.Text);
            xmlArg.SetAttribute("varid", variableData.DropdownInput.SelectedOption.Value);
            xmlMutation.AppendChild(xmlArg);
        }
        return xmlMutation;
    }
}
