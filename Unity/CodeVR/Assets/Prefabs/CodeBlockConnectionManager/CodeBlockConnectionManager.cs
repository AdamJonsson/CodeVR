using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEditor;

public class CodeBlockConnectionManager : MonoBehaviour
{
    [SerializeField] ConnectionLine _line;

    [SerializeField] private XRRayInteractor _leftController;

    [SerializeField] private XRRayInteractor _rightController;

    [Header("IF enabled, connection can be made by pressing the key (C). Must be disabled during production!")]
    [SerializeField] private bool _debugMode = false;

    private BlocklyCodeManager _blocklyCodeManager;

    public bool DebugMode { get => this._debugMode; }

    private CodeBlockManager _codeBlockManager;

    void Start()
    {
        this._leftController.selectExited.AddListener(OnDropBlock);
        this._rightController.selectExited.AddListener(OnDropBlock);

        this._blocklyCodeManager = FindObjectOfType<BlocklyCodeManager>();
        this._blocklyCodeManager.GenerateBlocklyCode();

        this._codeBlockManager = FindObjectOfType<CodeBlockManager>();
        if (this._debugMode) Debug.LogWarning("DEBUG MODE IS ENABLED FOR BLOCK CONNECTIONS. SHOULD BE DISABLED IF USING VR \n See the inspector for CodeBlockConnectionManager to change the settings");
    }

    void Update()
    {
        var bestPotentialConnection = this.GetBestPotentialConnection();
        this.DrawConnectionLine(bestPotentialConnection);

        if (this._debugMode) this.DebugActionUpdate();
    }

    private void DebugActionUpdate()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            this.ConnectBestPotentialConnection();
        }
    }

    private void OnDropBlock(SelectExitEventArgs args)
    {
        StartCoroutine(ConnectBestPotentialConnectionDelayed());
    }

    private IEnumerator ConnectBestPotentialConnectionDelayed()
    {
        yield return new WaitForSeconds(0.05f);
        this.ConnectBestPotentialConnection();
    }

    private void ConnectBestPotentialConnection()
    {
        var bestPotentialConnection = this.GetBestPotentialConnection();
        if (bestPotentialConnection == null) return;

        CodeBlockConnector inputConnector = bestPotentialConnection.Input;
        CodeBlockConnector outputConnector = bestPotentialConnection.Output;
        CodeBlock inputBlock = bestPotentialConnection.Input.BlockAttachedTo;
        CodeBlock outputBlock = bestPotentialConnection.Output.BlockAttachedTo;

        if (inputBlock.IsCurrentlyBeingMoved && !outputBlock.IsCurrentlyBeingMoved)
        {
            this.ConnectBlocks(fromConnector: outputConnector, toConnector: inputConnector);
            return;
        }
        
        if (!inputBlock.IsCurrentlyBeingMoved && outputBlock.IsCurrentlyBeingMoved)
        {
            this.ConnectBlocks(fromConnector: inputConnector, toConnector: outputConnector);
            return;
        }

        if (inputBlock.GetBlockCluster().Count > outputBlock.GetBlockCluster().Count)
        {
            this.ConnectBlocks(fromConnector: outputConnector, toConnector: inputConnector);
            return;
        }
        else
        {
            this.ConnectBlocks(fromConnector: inputConnector, toConnector: outputConnector);
            return;
        }
        
    }

    private void DrawConnectionLine(PotentialConnection potentialConnection)
    {
        if (potentialConnection != null) {
            this._line.SetPositions(potentialConnection.Input.DistanceReferencePoint.position, potentialConnection.Output.DistanceReferencePoint.position);
        }
        _line.gameObject.SetActive(potentialConnection != null);
    }

    private List<PotentialConnection> GetAllPotentialConnections()
    {
        var potentialConnections = new List<PotentialConnection>();
        foreach (var codeBlock in this._codeBlockManager.AllCodeBlocks)
        {
            potentialConnections.AddRange(codeBlock.GetAllPotentialConnections());
        }
        return potentialConnections;
    }

    private PotentialConnection GetBestPotentialConnection() 
    {
        var allPotentialConnections = GetAllPotentialConnections();
        if (allPotentialConnections.Count == 0) return null;
        
        float closestDistance = Mathf.Infinity;
        PotentialConnection closestConnection = null;

        foreach (var potentialConnection in allPotentialConnections)
        {
            if (closestDistance > potentialConnection.Distance)
            {
                closestDistance = potentialConnection.Distance;
                closestConnection = potentialConnection;
            }
        }

        return closestConnection;
    }

    private void ConnectBlocks(CodeBlockConnector fromConnector, CodeBlockConnector toConnector)
    {
        // Perform the basic connection
        fromConnector.Connect(connector: toConnector, snapConnectorsBlockCluster: false);
        toConnector.Connect(connector: fromConnector, snapConnectorsBlockCluster: true);

        // Notify the new block cluster of the connection. So it resize itself correctly
        toConnector.BlockAttachedTo.OnConnectionToThisBlock();

        // After the cluster have resized itself, we need to realign the blocks again
        toConnector.BlockAttachedTo.RealignBlockCluster();

        this._blocklyCodeManager.GenerateBlocklyCode();
    }

    private IEnumerator DelayedRealignBlocks(CodeBlockConnector connector)
    {
        yield return new WaitForSeconds(1.0f);
    }

    public void DetachConnector(CodeBlockConnector connectorToDetach)
    {
        var connectorOne = connectorToDetach;
        var connectorTwo = connectorToDetach.Connection;
        this.HandleDetachmentForConnector(connectorOne);
        this.HandleDetachmentForConnector(connectorTwo);

        this._blocklyCodeManager.GenerateBlocklyCode();
    }

    private void HandleDetachmentForConnector(CodeBlockConnector connector)
    {
        connector.Detach();
        connector.BlockAttachedTo.NotifyBlockClusterOfDetachement();
        connector.BlockAttachedTo.RealignBlockCluster();
    }


}
