using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CodeBlockManager : MonoBehaviour
{
    private List<CodeBlock> _allCodeBlocks = new List<CodeBlock>();
    public List<CodeBlock> AllCodeBlocks { get => this._allCodeBlocks; }

    private BlocklyCodeManager _blocklyCodeManager;

    public Action OnBlocksAddedOrDeleted;

    void Awake()
    {
        this._blocklyCodeManager = FindObjectOfType<BlocklyCodeManager>();
        this.LookForBlockAtStart();
    }

    private void LookForBlockAtStart()
    {
        this._allCodeBlocks = FindObjectsOfType<CodeBlock>().Where((block) => block.enabled).ToList();
        
        var helperBlocksFound = new List<CodeBlock>();
        foreach (var codeBlock in this._allCodeBlocks)
        {
            helperBlocksFound.AddRange(codeBlock.HelperBlocks);
        }
        this._allCodeBlocks.AddRange(helperBlocksFound);
        this.NotifyBlocksChanged();
    }

    public CodeBlock CreateNewBlock(CodeBlock original, Vector3 position, Quaternion rotation)
    {
        var createdBlock = Instantiate(original, position, rotation);
        this.AddBlockAndItsHelperBlocks(createdBlock);
        this._blocklyCodeManager.GenerateBlocklyCode();
        this.NotifyBlocksChanged();
        return createdBlock;
    }

    public CodeBlock CreateNewBlock(GameObject original, Vector3 position, Quaternion rotation)
    {
        var createdBlock = Instantiate(original, position, rotation);
        var codeBlock = createdBlock.GetComponent<CodeBlock>();
        this.AddBlockAndItsHelperBlocks(codeBlock);
        this._blocklyCodeManager.GenerateBlocklyCode();
        this.NotifyBlocksChanged();
        return codeBlock;
    }


    public void AddExistingBlock(List<CodeBlock> codeBlocks)
    {
        this.AddBlocksAndTheirHelperBlocks(codeBlocks);
        this._blocklyCodeManager.GenerateBlocklyCode();
        this.NotifyBlocksChanged();
    }

    private void AddBlocksAndTheirHelperBlocks(List<CodeBlock> codeBlocks)
    {
        foreach (var codeBlock in codeBlocks)
        {
            this.AddBlockAndItsHelperBlocks(codeBlock);
        }
        this.NotifyBlocksChanged();
    }

    private void AddBlockAndItsHelperBlocks(CodeBlock codeBlock)
    {
        this._allCodeBlocks.Add(codeBlock);
        this._allCodeBlocks.AddRange(codeBlock.HelperBlocks);
    }

    public void DeleteBlock(CodeBlock blockToDelete)
    {
        if (blockToDelete == null) return;
        this._allCodeBlocks.Remove(blockToDelete);
        foreach (var helperBlock in blockToDelete.HelperBlocks)
        {
            this._allCodeBlocks.Remove(helperBlock);
            foreach (var helperBlockChild in helperBlock.GetBlockCluster(includeSelf: false))
            {
                this._allCodeBlocks.Remove(helperBlockChild);
                Destroy(helperBlockChild.gameObject);
            }
            Destroy(helperBlock.gameObject);
        }

        Destroy(blockToDelete.gameObject);
        this._blocklyCodeManager.GenerateBlocklyCode();
        this.NotifyBlocksChanged();
    }

    public void RemoveAllBlocksInScene()
    {
        var codeBlocksToRemove = new List<CodeBlock>(this._allCodeBlocks);
        foreach (var codeBlock in codeBlocksToRemove)
        {
            this.DeleteBlock(codeBlock);
        }
        this.NotifyBlocksChanged();
    }

    private void NotifyBlocksChanged()
    {
        if (this.OnBlocksAddedOrDeleted != null)
            this.OnBlocksAddedOrDeleted.Invoke();
    }
}
