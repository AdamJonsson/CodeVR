using UnityEngine;

public class PotentialConnection {
    public CodeBlockConnector Output { get; set; }
    public CodeBlockConnector Input { get; set; }

    public float Distance {
        get => Vector3.Distance(Output.DistanceReferencePoint.position, Input.DistanceReferencePoint.position);
    }
}