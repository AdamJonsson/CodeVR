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

    void Awake()
    {
        this._collider = GetComponent<BoxCollider>();
        this._outputConnector = GetComponent<CodeBlockConnector>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var connectorEntered = other.GetComponent<CodeBlockConnector>();
        if (!IsInputConnectorValid(connectorEntered)) return;
        this._potentialConnections.Add(new PotentialConnection {
            Input = connectorEntered,
            Output = _outputConnector,
        });
        Debug.Log("New connector entered");
    }

    private void OnTriggerExit(Collider other)
    {
        var connectorExit = other.GetComponent<CodeBlockConnector>();
        if (connectorExit == null) return;
        var potentialConnectionToRemove = this._potentialConnections.Find((potentialConnection) => potentialConnection.Input == connectorExit);
        this._potentialConnections.Remove(potentialConnectionToRemove);
        Debug.Log("Connector exited");
    }

    private bool IsInputConnectorValid(CodeBlockConnector _inputConnector)
    {
        if (_inputConnector == null) return false;
        if (_inputConnector.IsConnected) return false;
        if (_inputConnector.BlockAttachedTo == this._outputConnector.BlockAttachedTo) return false;
        if (_potentialConnections.Find((potentialConnection) => potentialConnection.Input == _inputConnector) != null) return false;
        return true;
    }
}
