using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectAtStartManager : MonoBehaviour
{
    [SerializeField] private List<ConnectionAtStart> _connectionsAtStart;

    private CodeBlockConnectionManager _codeBlockConnectionManager;

    void Awake()
    {
        this._codeBlockConnectionManager = FindObjectOfType<CodeBlockConnectionManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ConnectBlocks(this._connectionsAtStart));
    }

    private IEnumerator ConnectBlocks(List<ConnectionAtStart> connectionsAtStart)
    {
        yield return new WaitForSeconds(0.1f);
        foreach (var connectionAtStart in connectionsAtStart)
        {
            yield return new WaitForSeconds(0.1f);
            this._codeBlockConnectionManager.ConnectBlocks(connectionAtStart.From, connectionAtStart.To);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        var index = 0;
        foreach (var connection in this._connectionsAtStart)
        {
            // Uncomment this for a better debugging experience 
            // Handles.Label(
            //     connection.From.DistanceReferencePoint.position - (connection.From.DistanceReferencePoint.position - connection.To.DistanceReferencePoint.position) / 2, 
            //     "C: " + index
            // );
            Gizmos.DrawLine(connection.From.DistanceReferencePoint.position, connection.To.DistanceReferencePoint.position);
            index++;
        }
    }
    
}

[Serializable]
public class ConnectionAtStart
{
    public CodeBlockConnector From; 
    public CodeBlockConnector To; 
}
