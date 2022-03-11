using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlocklyCodeManager : MonoBehaviour
{
    public void GenerateBlocklyCode()
    {
        var codeBlockManager = FindObjectOfType<CodeBlockManager>();
        var blockClusters = new List<CodeBlock>();

        foreach (var block in codeBlockManager.AllCodeBlocks)
        {
            if (block.IsRootBlock)
                blockClusters.Add(block);
        }

        var blocklyString = BlocklyXMLGenerator.CreateXMLStringFromRootBlocks(blockClusters);
        StartCoroutine(
            WebsiteConnection.UpdateBlocklyCode(blocklyString)
        );
        Debug.Log("Number of blocks: " + blockClusters.Count);
        Debug.Log(blocklyString);
    }
}
