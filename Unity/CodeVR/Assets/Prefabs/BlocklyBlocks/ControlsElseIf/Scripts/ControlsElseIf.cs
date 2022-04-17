using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsElseIf : MonoBehaviour
{
    [SerializeField] private CodeBlockConnector _ifConnector;
    [SerializeField] private CodeBlockConnector _doConnector;

    public void SetDoAndIfXmlData(int indexOfElseIfs)
    {
        this._ifConnector.UpdateBlocklyConnectionSetting("value", "IF" + (indexOfElseIfs + 1));
        this._doConnector.UpdateBlocklyConnectionSetting("statement", "DO" + (indexOfElseIfs + 1));
    }
}
