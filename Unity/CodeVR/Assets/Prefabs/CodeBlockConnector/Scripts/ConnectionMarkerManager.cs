using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ConnectionMarkerManager : MonoBehaviour
{
    [SerializeField] private CodeBlockConnector _connector;

    [SerializeField] private GameObject _rowInputConnectorObject;
    [SerializeField] private GameObject _rowOutputConnectorObject;
    [SerializeField] private GameObject _columnInputConnectorObject;
    [SerializeField] private GameObject _columnOutputConnectorObject;

    void Update()
    {
        if (!Application.isPlaying) this.ApplyConnectorObjectVisibility();
    }

    private void ApplyConnectorObjectVisibility()
    {
        ShowWhenConditionIsMet(_rowInputConnectorObject, CodeBlockConnector.Types.Input, CodeBlockConnector.Categories.Row);
        ShowWhenConditionIsMet(_rowOutputConnectorObject, CodeBlockConnector.Types.Output, CodeBlockConnector.Categories.Row);
        ShowWhenConditionIsMet(_columnInputConnectorObject, CodeBlockConnector.Types.Input, CodeBlockConnector.Categories.Column);
        ShowWhenConditionIsMet(_columnOutputConnectorObject, CodeBlockConnector.Types.Output, CodeBlockConnector.Categories.Column);
    }

    private void ShowWhenConditionIsMet(GameObject marker, CodeBlockConnector.Types connectionType, CodeBlockConnector.Categories connectionCategory)
    {
        marker.SetActive(_connector.ConnectionType == connectionType && _connector.ConnectionCategory == connectionCategory);
    }
}
