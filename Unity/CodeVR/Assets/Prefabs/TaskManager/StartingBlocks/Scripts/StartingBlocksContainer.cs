using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingBlocksContainer : MonoBehaviour
{
    [SerializeField] private List<CodeBlock> _codeBlocks = new List<CodeBlock>();
    public List<CodeBlock> CodeBlocks { get => this._codeBlocks; } 

    [SerializeField] private List<ConnectionAtStart> _connectionsAtStart;
    public List<ConnectionAtStart> ConnectionsAtStart { get => this._connectionsAtStart; } 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

[Serializable]
public class ConnectionAtStart
{
    public CodeBlockConnector From; 
    public CodeBlockConnector To; 
}