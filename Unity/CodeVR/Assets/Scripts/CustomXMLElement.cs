using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public abstract class CustomXMLElement : MonoBehaviour
{
    protected virtual void Start(){}

    public abstract XmlElement GetXmlElement(XmlDocument document);
}
