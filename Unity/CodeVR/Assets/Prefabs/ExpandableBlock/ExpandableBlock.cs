using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ExpandableBlock : MonoBehaviour
{
    [Header("Scaling Values")]
    [SerializeField] private Vector3 _expandScale = Vector3.one;

    [Tooltip("Values in vector should vary between 1 and -1")]
    [SerializeField] private Vector3 _expandAnchor = Vector3.zero;

    [Header("Objects References")]
    [SerializeField] private List<ExpandableSetting> _expandables = new List<ExpandableSetting>();

    [Header("Automatic Scale Settings")]
    [SerializeField] private CodeBlockConnector _connectorEffectsHeight;
    [SerializeField] private CodeBlockConnector _connectorEffectsWidth;

    [Tooltip("The min expand scale, the block scale can not be less than this vector")]
    [SerializeField] private Vector3 _minExpandSize;

    [Tooltip("Can be set if there are blocks that exist outside the expandable block")]
    [SerializeField] private Vector3 _extraStaticSize;

    [Tooltip("Can be used to get extra padding for a new block connection")]
    [SerializeField] private Vector3 _extraExpandSize;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void ApplyScaleToExpandableSettings()
    {
        foreach (var expandable in this._expandables)
        {
            if (expandable.ShouldScale)
                expandable.TransformReference.localScale = this._expandScale;

            expandable.TransformReference.localPosition = expandable.Offset + Vector3.Scale(this._expandScale - Vector3.one, this._expandAnchor + expandable.ScaleOffset) / 2.0f;
        }
    }

    public void ChangeSizeFromConnectors()
    {
        if (this._connectorEffectsHeight != null)
            this._expandScale.y = GetSizeOfClusterConnectedToConnector(this._connectorEffectsHeight).y;
        if (this._connectorEffectsWidth != null)
            this._expandScale.x = GetSizeOfClusterConnectedToConnector(this._connectorEffectsWidth).x;
        this.ApplyScaleToExpandableSettings();
    }

    private Vector3 GetSizeOfClusterConnectedToConnector(CodeBlockConnector connector)
    {
        var connectorSize = Vector3.zero;
        if (connector != null && connector.IsConnected)
        {
            connectorSize += new Vector3(connector.BlockConnectedTo.Size.WidthOfBlocksRight(), connector.BlockConnectedTo.Size.HeightOfBlocksDown(), 1.0f);
        }

        return new Vector3(
            Mathf.Max(connectorSize.x, this._minExpandSize.x),
            Mathf.Max(connectorSize.y, this._minExpandSize.y),
            1.0f
        ) + this._extraExpandSize;
    }

    public float GetHeight()
    {
        var heightOfChildren = this.GetSizeOfClusterConnectedToConnector(this._connectorEffectsHeight).y;
        return heightOfChildren + this._extraStaticSize.y;
    }

    public float GetWidth()
    {
        var widthOfChildren = this.GetSizeOfClusterConnectedToConnector(this._connectorEffectsWidth).x;
        return widthOfChildren + this._extraStaticSize.x;
    }

    [Serializable]
    public struct ExpandableSetting
    {
        public Transform TransformReference;

        public Vector3 Offset;

        public Vector3 ScaleOffset;

        public bool ShouldScale;

    }
}
