using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingBlocksContainer : MonoBehaviour
{
    [SerializeField] private List<CodeBlock> _codeBlocks = new List<CodeBlock>();
    public List<CodeBlock> CodeBlocks { get => this._codeBlocks; } 
}

