using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CodeBlockManager : MonoBehaviour
{
    private List<CodeBlock> _allCodeBlocks = new List<CodeBlock>();
    public List<CodeBlock> AllCodeBlocks { get => this._allCodeBlocks; }

    private BlocklyCodeManager _blocklyCodeManager;

    // Start is called before the first frame update
    void Start()
    {
        this._blocklyCodeManager = FindObjectOfType<BlocklyCodeManager>();
        this._allCodeBlocks = FindObjectsOfType<CodeBlock>().Where((block) => block.enabled == true).ToList();
    }

    public CodeBlock CreateNewBlock(CodeBlock original, Vector3 position, Quaternion rotation)
    {
        var createdBlock = Instantiate(original, position, rotation);
        this._allCodeBlocks.Add(createdBlock);
        this._blocklyCodeManager.GenerateBlocklyCode();
        return createdBlock;
    }

    public void DeleteBlock(CodeBlock blockToDelete)
    {
        if (blockToDelete == null) return;
        this._allCodeBlocks.Remove(blockToDelete);
        Destroy(blockToDelete.gameObject);
        this._blocklyCodeManager.GenerateBlocklyCode();
    }
}
