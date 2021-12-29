using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CodeBlockConnectionManager : MonoBehaviour
{
    private List<CodeBlock> _allCodeBlocks = new List<CodeBlock>();

    // Start is called before the first frame update
    void Start()
    {
        this._allCodeBlocks = FindObjectsOfType<CodeBlock>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        var bestPotentialConnection = this.GetBestPotentialConnection();
        this.DrawConnectionLine(bestPotentialConnection);
    }

    private void DrawConnectionLine(PotentialConnection potentialConnection)
    {
        if (potentialConnection == null) return;
        Debug.DrawLine(
            potentialConnection.Input.DistanceReferencePoint.position, 
            potentialConnection.Output.DistanceReferencePoint.position
        );
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
}
