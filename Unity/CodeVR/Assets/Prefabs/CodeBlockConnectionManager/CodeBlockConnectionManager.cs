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

    private List<CodeBlock> _allCodeBlocks = new List<CodeBlock>();

    public bool DebugMode { get => this._debugMode; }

    void Start()
    {
        this._allCodeBlocks = FindObjectsOfType<CodeBlock>().ToList();
        this._leftController.selectExited.AddListener(OnDropBlock);
        this._rightController.selectExited.AddListener(OnDropBlock);

        // StartCoroutine(
        //     WebsiteConnection.UpdateBlocklyCode("<xml xmlns='https://developers.google.com/blockly/xml'> <block type='controls_if' id='JK8L=R)FBxLJX@WmOv^J' x='77' y='65'> <statement name='DO0'> <block type='controls_if' id='l5dvRlyoDlJ;|1,=!4,0'> <statement name='DO0'> <block type='controls_if' id='Y$^Gn{IS0XtE_Sfq!lrW'></block> </statement> <next> <block type='controls_if' id='J58W)3Y{wVGhat,+`KqC'> <statement name='DO0'> <block type='controls_if' id='us/+CiQqRRhM1Q{z`|+A'></block> </statement> </block> </next> </block> </statement> </block> </xml>")
        // );

        Debug.Log(BlocklyXMLGenerator.CreateXMLStringFromRootBlock(new CodeBlock()));
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
        foreach (var codeBlock in this._allCodeBlocks)
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
    }

    private IEnumerator DelayedRealignBlocks(CodeBlockConnector connector)
    {
        yield return new WaitForSeconds(1.0f);
    }

    public void DetachConnector(CodeBlockConnector connectorToDetach)
    {
        var connectorOne = connectorToDetach;
        var connectorTwo = connectorToDetach.Connection;
        connectorOne.Detach();
        connectorTwo.Detach();
        connectorOne.BlockAttachedTo.NotifyBlockClusterOfDetachement();
        connectorTwo.BlockAttachedTo.NotifyBlockClusterOfDetachement();

        connectorOne.BlockAttachedTo.RealignBlockCluster();
        connectorTwo.BlockAttachedTo.RealignBlockCluster();
    }
}
