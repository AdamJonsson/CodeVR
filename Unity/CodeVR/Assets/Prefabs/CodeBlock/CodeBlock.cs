using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeBlock : MonoBehaviour
{
    [SerializeField] private List<CodeBlockConnector> _connectors;
    private CodeBlockConnector _outputConnector;
    private List<CodeBlockConnector> _inputConnectors;

    private InputFinder _inputFinder;

    void Start()
    {
        this.SetupConnectors();
        this._inputFinder = this._outputConnector.GetComponent<InputFinder>();
    }

    private void SetupConnectors()
    {
        this._outputConnector = this._connectors.Find((connector) => connector.ConnectionType == CodeBlockConnector.Types.Output);
        this._inputConnectors = this._connectors.FindAll((connector) => connector.ConnectionType == CodeBlockConnector.Types.Input);
    }

    public List<PotentialConnection> GetAllPotentialConnections()
    {
        return this._inputFinder.PotentialConnections;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
