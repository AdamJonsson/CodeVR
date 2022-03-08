

using System.Xml;

public class BlocklyXMLGenerator 
{
    public static string CreateXMLStringFromRootBlock(CodeBlock rootBlock)
    {
        XmlDocument document = new XmlDocument();

        //xml declaration is recommended, but not mandatory
        XmlDeclaration xmlDeclaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);

        XmlElement rootnode = document.CreateElement("xml");
        rootnode.SetAttribute("xmlns", "https://developers.google.com/blockly/xml");

        //create the root element
        document.InsertBefore(xmlDeclaration, rootnode);

        var xmlElementFromRootBlock = CreateXMLElementFromBlock(document, rootBlock);
        rootnode.AppendChild(xmlElementFromRootBlock);

        document.AppendChild(rootnode);
        return document.OuterXml;
    }

    public static XmlElement CreateXMLElementFromBlock(XmlDocument document, CodeBlock block)
    {
        XmlElement xmlBlockElement = document.CreateElement("block");
        xmlBlockElement.SetAttribute("type", block.BlocklyTypeString);
        
        
        return xmlBlockElement;
    }
}