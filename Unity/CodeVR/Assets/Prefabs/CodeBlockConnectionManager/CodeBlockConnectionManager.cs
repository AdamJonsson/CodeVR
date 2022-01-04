using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlockConnectionManager : MonoBehaviour
{
    [SerializeField] ConnectionLine _line;

    [SerializeField] private XRRayInteractor _leftController;

    [SerializeField] private XRRayInteractor _rightController;

    private List<CodeBlock> _allCodeBlocks = new List<CodeBlock>();

    void Start()
    {
        this._allCodeBlocks = FindObjectsOfType<CodeBlock>().ToList();
        this._leftController.selectExited.AddListener(OnDropBlock);
        this._rightController.selectExited.AddListener(OnDropBlock);
    }

    void Update()
    {
        var bestPotentialConnection = this.GetBestPotentialConnection();
        this.DrawConnectionLine(bestPotentialConnection);
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
        fromConnector.Connect(connector: toConnector, snapConnectorsBlockCluster: false);
        toConnector.Connect(connector: fromConnector, snapConnectorsBlockCluster: true);
        toConnector.BlockAttachedTo.OnConnection();
    }

    public void DetachConnector(CodeBlockConnector connectorToDetach)
    {
        var connectorOne = connectorToDetach;
        var connectorTwo = connectorToDetach.Connection;
        connectorOne.Detach();
        connectorTwo.Detach();
    }
}
