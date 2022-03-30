

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BlocklyXMLGenerator 
{
    public static string CreateXMLStringFromRootBlocks(List<CodeBlock> blocks, List<VariableDeclaration> variableDeclarations)
    {
        XmlDocument document = new XmlDocument();

        //xml declaration is recommended, but not mandatory
        XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);

        XmlElement rootnode = document.CreateElement("xml");
        rootnode.SetAttribute("xmlns", "https://developers.google.com/blockly/xml");

        // Create the xml for all the variable decleration
        XmlElement xmlVariableContainer = document.CreateElement("variables");
        foreach (var variable in variableDeclarations)
        {
            XmlElement xmlVariable = document.CreateElement("variable");
            xmlVariable.SetAttribute("id", variable.ID);
            xmlVariable.InnerText = variable.Name;
            xmlVariableContainer.AppendChild(xmlVariable);
        }
        rootnode.AppendChild(xmlVariableContainer);

        // Create the xml for all the blocks
        foreach (var block in blocks)
        {
            var xmlElementFromRootBlock = CreateXMLElementFromBlock(document, block);
            rootnode.AppendChild(xmlElementFromRootBlock);
        }

        document.AppendChild(rootnode);
        Debug.Log(document.OuterXml);
        return document.OuterXml;
    }

    public static XmlElement CreateXMLElementFromBlock(XmlDocument document, CodeBlock block)
    {
        XmlElement xmlBlockElement = document.CreateElement("block");
        xmlBlockElement.SetAttribute("type", block.BlocklyTypeString);
        xmlBlockElement.SetAttribute("id", block.ID);

        // Add fields
        foreach (var field in block.BlocklyFields)
        {
            XmlElement fieldElement = document.CreateElement("field");
            fieldElement.SetAttribute("name", field.Name);
            fieldElement.InnerText = field.Value;
            xmlBlockElement.AppendChild(fieldElement);
        }

        // Add curstom xml elements from block
        foreach (var customXMLElement in block.CustomXmlElements)
        {
            var xmlElement = customXMLElement.GetXmlElement(document);
            if (xmlElement == null) continue;
            xmlBlockElement.AppendChild(xmlElement);
        }
        
        // Add connected blocks to XML Element
        foreach (var connector in block.AllConnectors)
        {
            if (connector.ConnectionFlow == CodeBlockConnector.Flows.Previous) continue;
            if (!connector.IsConnected) continue;
            if (connector.BlockConnectedTo.ExcludeInAutomaticBlocklyCodeGeneration) continue;

            XmlElement connectorElement = document.CreateElement(connector.XmlTag);
            if (connector.NameAttributeValue != null)
                connectorElement.SetAttribute("name", connector.NameAttributeValue);

            connectorElement.AppendChild(CreateXMLElementFromBlock(document, connector.BlockConnectedTo));
            xmlBlockElement.AppendChild(connectorElement);
        }
        
        return xmlBlockElement;
    }
}