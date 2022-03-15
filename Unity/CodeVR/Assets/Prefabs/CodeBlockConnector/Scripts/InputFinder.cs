using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(CodeBlockConnector))]
public class InputFinder : MonoBehaviour
{
    private BoxCollider _collider;

    private List<PotentialConnection> _potentialConnections = new List<PotentialConnection>();

    private CodeBlockConnector _outputConnector;

    public List<PotentialConnection> PotentialConnections { get => _potentialConnections; }

    private CodeBlockConnectionManager _connectionManager;

    void Awake()
    {
        this._collider = GetComponent<BoxCollider>();
        this._outputConnector = GetComponent<CodeBlockConnector>();
    }

    void Start()
    {
        this._connectionManager = FindObjectOfType<CodeBlockConnectionManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var connectorEntered = other.GetComponent<CodeBlockConnector>();
        if (InputConnectorAlreadyExist(connectorEntered)) return;
        if (!IsInputConnectorValid(connectorEntered)) return;

        this._potentialConnections.Add(new PotentialConnection {
            Input = connectorEntered,
            Output = _outputConnector
        });
    }

    private void OnTriggerExit(Collider other)
    {
        var connectorExit = other.GetComponent<CodeBlockConnector>();
        if (connectorExit == null) return;
        var potentialConnectionToRemove = this._potentialConnections.Find((potentialConnection) => potentialConnection.Input == connectorExit);
        this._potentialConnections.Remove(potentialConnectionToRemove);
    }

    private bool IsInputConnectorValid(CodeBlockConnector inputConnector)
    {
        if (inputConnector == null) return false;
        if (inputConnector.IsConnected || this._outputConnector.IsConnected) return false;
        if (inputConnector.ConnectionType == CodeBlockConnector.Types.Output) return false;
        if (inputConnector.ConnectionCategory != _outputConnector.ConnectionCategory) return false;

        // At least one controller need to have the block grabbed
        if (!_outputConnector.BlockAttachedTo.IsCurrentlyBeingMoved && !inputConnector.BlockAttachedTo.IsCurrentlyBeingMoved && !this._connectionManager.DebugMode) return false;

        // If the potential input connector is part of the same block
        if (inputConnector.BlockAttachedTo == this._outputConnector.BlockAttachedTo) return false;
        
        // If the potential input connector is part of the same block cluster as this output connector
        if (inputConnector.BlockAttachedTo.BlockIsPartOfCluster(_outputConnector.BlockAttachedTo)) return false;
        
        return true;
    }

    private bool InputConnectorAlreadyExist(CodeBlockConnector inputConnector)
    {
        return _potentialConnections.Find((potentialConnection) => potentialConnection.Input == inputConnector) != null;
    }

    public void ClearPotentialConnections()
    {
        _potentialConnections.Clear();
    }
}
