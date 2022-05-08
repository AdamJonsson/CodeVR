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

    private List<PotentialConnection> _currentPotentialConnections = new List<PotentialConnection>();

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
        this._currentPotentialConnections = GetAllPotentialConnections(true);
        this.DrawConnectionLine();
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

    private void DrawConnectionLine()
    {
        PotentialConnection connectionToRender = null;

        var bestCompatiblePotentialConnections = this.GetBestPotentialConnection();
        if (bestCompatiblePotentialConnections != null) {
            connectionToRender = bestCompatiblePotentialConnections;
        }

        if (connectionToRender == null)
        {
            connectionToRender = this.GetBestPotentialConnection(includeIncompatibleConnection: true);
        }

        if (connectionToRender != null)
        {
            this._line.SetPositions(
                connectionToRender.Input.DistanceReferencePoint.position, 
                connectionToRender.Output.DistanceReferencePoint.position
            );
            this._line.ConnectionCompatible(connectionToRender.IsCategoryCompatible);
        }
        
        this._line.gameObject.SetActive(connectionToRender != null);
    }

    private List<PotentialConnection> GetAllPotentialConnections(bool includeIncompatibleConnections = false)
    {
        var potentialConnections = new List<PotentialConnection>();
        foreach (var codeBlock in this._codeBlockManager.AllCodeBlocks)
        {
            potentialConnections.AddRange(codeBlock.GetAllPotentialConnections(includeIncompatibleConnections));
        }
        return potentialConnections;
    }

    private PotentialConnection GetBestPotentialConnection(bool includeIncompatibleConnection = false) 
    {
        List<PotentialConnection> allPotentialConnectionsToCompare;
        if (includeIncompatibleConnection)
            allPotentialConnectionsToCompare = this._currentPotentialConnections;
        else
            allPotentialConnectionsToCompare = this._currentPotentialConnections.Where((potentialConnection) => potentialConnection.IsCategoryCompatible == true).ToList();

        if (allPotentialConnectionsToCompare.Count == 0) return null;
        
        float closestDistance = Mathf.Infinity;
        PotentialConnection closestConnection = null;

        foreach (var potentialConnection in allPotentialConnectionsToCompare)
        {
            if (closestDistance > potentialConnection.Distance)
            {
                closestDistance = potentialConnection.Distance;
                closestConnection = potentialConnection;
            }
        }

        return closestConnection;
    }

    public void ConnectBlocks(CodeBlockConnector fromConnector, CodeBlockConnector toConnector, bool quiet = false)
    {
        // Perform the basic connection
        fromConnector.Connect(connector: toConnector, snapConnectorsBlockCluster: false);
        toConnector.Connect(connector: fromConnector, snapConnectorsBlockCluster: true);

        // Make the from-connector to have the same "active" state as the to-connector
        fromConnector.BlockAttachedTo.gameObject.SetActive(toConnector.BlockAttachedTo.gameObject.activeSelf);

        // Notify the new block cluster of the connection. So it resize itself correctly
        toConnector.BlockAttachedTo.OnConnectionToThisBlock();

        // After the cluster have resized itself, we need to realign the blocks again
        toConnector.BlockAttachedTo.RealignBlockCluster();

        if (!quiet)
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
