using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CodeBlock))]
public class CodeBlockSize : MonoBehaviour
{
    [SerializeField] private Vector3 _staticSize = Vector3.one;

    [Header("Height")]
    [SerializeField] private CalculationMode _heightCalculationMode = CalculationMode.Largest;
    [SerializeField] private List<ExpandableBlock> _heightExpandableBlocks = new List<ExpandableBlock>();
    [SerializeField] private List<CodeBlockConnector> _connectorsThatEffectHeight = new List<CodeBlockConnector>();

    [Header("Width")]
    [SerializeField] private CalculationMode _widthCalculationMode = CalculationMode.Largest;
    [SerializeField] private List<ExpandableBlock> _widthExpandableBlocks = new List<ExpandableBlock>();
    [SerializeField] private List<CodeBlockConnector> _connectorsThatEffectWidth = new List<CodeBlockConnector>();

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
                var width = expandableBlock.GetWidth();
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
            expandableBlock.ChangeSizeFromConnectors();
        }
    }

    /// <summary>This is the height of the block itself and every block that is connected below. The distance between blocks are also included.</summary>
    public float HeightOfBlocksDown()
    {
        var height = Height;
        foreach (var connector in this._connectorsThatEffectHeight)
        {
            if (!connector.IsConnected) continue;
            height += connector.BlockConnectedTo.Size.HeightOfBlocksDown() + connector.ConnectionDistance;
        }
        return height;
    }

    /// <summary>This is the width of the block itself and every block that is connected to the right. The distance between blocks are also included.</summary>
    public float WidthOfBlocksRight()
    {
        var width = Width;
        foreach (var connector in this._connectorsThatEffectWidth)
        {
            if (!connector.IsConnected) continue;
            width += connector.BlockConnectedTo.Size.WidthOfBlocksRight() + connector.ConnectionDistance;
        }
        return width;
    }

    // Start is called before the first frame update
    void Start()
    {
        this._codeBlock = GetComponent<CodeBlock>();
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
