using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CodeBlock))]
public class CodeBlockSize : MonoBehaviour
{
    [SerializeField] private Vector3 _staticSize = Vector3.one;

    [Header("--Height--")]
    [SerializeField] private CalculationMode _heightCalculationMode = CalculationMode.Largest;
    [SerializeField] private List<ExpandableBlock> _heightExpandableBlocks = new List<ExpandableBlock>();
    public List<ExpandableBlock> HeightExpandableBlocks => this._heightExpandableBlocks;
    
    [SerializeField] private List<CodeBlockConnector> _connectorsThatEffectHeight = new List<CodeBlockConnector>();
    public List<ExpandableBlock> WidthExpandableBlocks => this._widthExpandableBlocks;

    [Header("--Width Expandables--")]
    [SerializeField] private CalculationMode _widthCalculationMode = CalculationMode.Largest;
    [SerializeField] private List<ExpandableBlock> _widthExpandableBlocks = new List<ExpandableBlock>();
    
    [Header("--Width Connectors--")]
    [SerializeField] private CalculationMode _widthConnectorsCalculationMode = CalculationMode.Additive;
    [SerializeField] private List<CodeBlockConnector> _connectorsThatEffectWidth = new List<CodeBlockConnector>();
    public List<CodeBlockConnector> ConnectorsThatEffectWidth { get => this._connectorsThatEffectWidth; set => this._connectorsThatEffectWidth = value; }

    [Header("--All Connectors--")]
    [SerializeField] private Vector3 _marginWhenNoConnectionExist = Vector3.zero;

    private CodeBlock _codeBlock;

    public float Height
    {
        get 
        {
            var sum = 0.0f;
            var max = 0.0f;
            foreach (var expandableBlock in this._heightExpandableBlocks)
            {
                var height = expandableBlock.GetHeight();
                sum += height;
                if (max < height) max = height;
            }
            return (this._heightCalculationMode == CalculationMode.Additive ? sum : max) + this._staticSize.y;
        }
    }

    public float Width
    {
        get 
        {
            var sum = 0.0f;
            var max = 0.0f;
            foreach (var expandableBlock in this._widthExpandableBlocks)
            {
                var width = expandableBlock.GetWidth(this._codeBlock);
                sum += width;
                if (max < width) max = width;
            }
            return (this._widthCalculationMode == CalculationMode.Additive ? sum : max) + this._staticSize.x;
        }
    }

    public List<ExpandableBlock> AllExpandableBlocks { 
        get
        {
            var allBlocks = new List<ExpandableBlock>();
            allBlocks.AddRange(this._widthExpandableBlocks);
            allBlocks.AddRange(this._heightExpandableBlocks);
            return allBlocks;
        }
    }

    public void ResizeAllExpandableBlocks()
    {
        foreach (var expandableBlock in this.AllExpandableBlocks)
        {
            expandableBlock.ChangeSizeFromConnectorsAndInput(this._codeBlock);
        }
    }

    /// <summary>This is the height of the block itself and every block that is connected below. The distance between blocks are also included.</summary>
    public float HeightOfBlocksDown()
    {
        var height = this.Height;
        foreach (var connector in this._connectorsThatEffectHeight)
        {
            if (!connector.IsConnected) 
                height += _marginWhenNoConnectionExist.y;
            else
                height += connector.BlockConnectedTo.Size.HeightOfBlocksDown() + connector.ConnectionDistance;
        }
        return height;
    }

    /// <summary>This is the width of the block itself and every block that is connected to the right. The distance between blocks are also included.</summary>
    public float WidthOfBlocksRight()
    {
        var sum = 0.0f;
        var max = 0.0f;
        foreach (var connector in this._connectorsThatEffectWidth)
        {
            if (!connector.IsConnected)
            {
                sum += this._marginWhenNoConnectionExist.x;
                max = Mathf.Max(max, this._marginWhenNoConnectionExist.x);
            }
            else
            {
                var size = connector.BlockConnectedTo.Size.WidthOfBlocksRight() + connector.ConnectionDistance;
                sum += size;
                max = Mathf.Max(max, size);
            }
        }

        return (this._widthConnectorsCalculationMode == CalculationMode.Additive ? sum : max) + this.Width;
    }

    private void OnInputsChangeThatEffectWidth(string value)
    {
        StartCoroutine(ResizeAndRealignAllBlocksDelayed());
    }

    private IEnumerator ResizeAndRealignAllBlocksDelayed()
    {
        // The reason why we have a short delay is that the canvas needs time to
        // update its size from the new value.
        yield return new WaitForSeconds(0.05f);

        var blocks = this._codeBlock.GetBlockCluster(true);
        foreach (var block in blocks)
        {
            block.Size.ResizeAllExpandableBlocks();
        }
        this._codeBlock.RealignBlockCluster();
    }

    // Start is called before the first frame update
    void Start()
    {
        this._codeBlock = GetComponent<CodeBlock>();
        foreach (var expandableBlock in this.AllExpandableBlocks)
        {
            if (expandableBlock.InputFieldEffectsWidth == null) continue;
            expandableBlock.InputFieldEffectsWidth.OnChange += this.OnInputsChangeThatEffectWidth;
        }

        // Resize 
        StartCoroutine(this.ResizeAndRealignAllBlocksDelayed());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum CalculationMode 
    {
        Additive,
        Largest
    }
}
