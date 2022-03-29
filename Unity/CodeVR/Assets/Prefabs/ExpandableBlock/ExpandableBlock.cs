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
    [SerializeField] private CodeBlockSize.CalculationMode _heightCalculationMode = CodeBlockSize.CalculationMode.Additive;
    [SerializeField] private List<CodeBlockConnector> _connectorsEffectsHeight = new List<CodeBlockConnector>();
    [SerializeField] private CodeBlockSize.CalculationMode _widthCalculationMode = CodeBlockSize.CalculationMode.Additive;
    [SerializeField] private List<CodeBlockConnector> _connectorsEffectsWidth = new List<CodeBlockConnector>();

    [SerializeField] private InputBase _inputFieldEffectsWidth;
    public InputBase InputFieldEffectsWidth { get => this._inputFieldEffectsWidth; }


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

    void Update()
    {
        if (!Application.isPlaying) this.ApplyScaleToExpandableSettings();
    }

    private void ApplyScaleToExpandableSettings()
    {
        foreach (var expandable in this._expandables)
        {
            if (expandable.ShouldScale)
                expandable.TransformReference.localScale = Vector3.Scale(this._expandScale, expandable.ScaleFactor) + expandable.ExtraStaticScale;

            expandable.TransformReference.localPosition = 
                expandable.Offset +
                Vector3.Scale(this._expandScale - Vector3.one, this._expandAnchor + expandable.ScaleOffset)
                / 2.0f;
        }
    }

    private float GetWidthFromInput(CodeBlock codeBlock)
    {
        if (_inputFieldEffectsWidth == null) return 0.0f;
        var inputRectTransform = this._inputFieldEffectsWidth.RectTransform;
        var widthOfInputField = inputRectTransform.rect.width * inputRectTransform.lossyScale.x / codeBlock.transform.localScale.x;
        return Mathf.Max(widthOfInputField, this._minExpandSize.x) + this._extraExpandSize.x;
    }

    public void ChangeSizeFromConnectorsAndInput(CodeBlock codeBlock)
    {
        if (this._connectorsEffectsHeight.Count > 0)
            this._expandScale.y = GetSizeOfClusterConnectedToConnectors(this._connectorsEffectsHeight).y;
        
        var width = 0.0f;
        if (this._connectorsEffectsWidth.Count > 0)
            width += GetSizeOfClusterConnectedToConnectors(this._connectorsEffectsWidth).x;

        if (this._inputFieldEffectsWidth != null)
            width += this.GetWidthFromInput(codeBlock);

        if (width > 0.0f)
            this._expandScale.x = width;

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

    private Vector3 GetSizeOfClusterConnectedToConnectors(List<CodeBlockConnector> connectors)
    {
        Vector3 size = new Vector3(0.0f, 0.0f, 1.0f);

        foreach (var connector in connectors)
        {
            var sizeOfConnector = this.GetSizeOfClusterConnectedToConnector(connector);
            if (this._widthCalculationMode == CodeBlockSize.CalculationMode.Additive)
            {
                size.x += sizeOfConnector.x;
            }
            else
            {
                size.x = Mathf.Max(size.x, sizeOfConnector.x);
            }

            if (this._heightCalculationMode == CodeBlockSize.CalculationMode.Additive)
            {
                size.y += sizeOfConnector.y;
            }
            else
            {
                size.y = Mathf.Max(size.y, sizeOfConnector.y);
            }
        }
        return size;
    }

    public float GetHeight()
    {
        var heightOfChildren = this.GetSizeOfClusterConnectedToConnectors(this._connectorsEffectsHeight).y;
        return heightOfChildren + this._extraStaticSize.y;
    }

    public float GetWidth(CodeBlock codeBlock)
    {
        var widthOfChildren = this.GetSizeOfClusterConnectedToConnectors(this._connectorsEffectsWidth).x + this.GetWidthFromInput(codeBlock);
        return widthOfChildren + this._extraStaticSize.x;
    }

    [Serializable]
    public struct ExpandableSetting
    {
        public Transform TransformReference;

        public Vector3 Offset;

        public Vector3 ScaleOffset;

        public Vector3 ScaleFactor;

        public Vector3 ExtraStaticScale;

        public bool ShouldScale;

    }

}
