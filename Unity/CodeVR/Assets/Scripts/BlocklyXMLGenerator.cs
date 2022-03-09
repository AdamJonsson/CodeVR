

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class BlocklyXMLGenerator 
{
    public static string CreateXMLStringFromRootBlocks(List<CodeBlock> blocks)
    {
        XmlDocument document = new XmlDocument();

        //xml declaration is recommended, but not mandatory
        XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);

        XmlElement rootnode = document.CreateElement("xml");
        rootnode.SetAttribute("xmlns", "https://developers.google.com/blockly/xml");

        //create the root element
        // document.InsertBefore(xmlDeclaration, rootnode);

        foreach (var block in blocks)
        {
            var xmlElementFromRootBlock = CreateXMLElementFromBlock(document, block);
            rootnode.AppendChild(xmlElementFromRootBlock);
        }

        document.AppendChild(rootnode);
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
            fieldElement.InnerText = field.DropdownInput.Value;
            xmlBlockElement.AppendChild(fieldElement);
        }
        
        // Add connected blocks to XML Element
        foreach (var connector in block.AllConnectors)
        {
            if (connector.ConnectionFlow == CodeBlockConnector.Flows.Previous) continue;
            if (!connector.IsConnected) continue;

            XmlElement connectorElement = document.CreateElement(connector.XmlTag);
            if (connector.NameAttributeValue != null)
                connectorElement.SetAttribute("name", connector.NameAttributeValue);

            connectorElement.AppendChild(CreateXMLElementFromBlock(document, connector.BlockConnectedTo));
            xmlBlockElement.AppendChild(connectorElement);
        }
        
        return xmlBlockElement;
    }
}