using UnityEngine;

public class PotentialConnection {
    public CodeBlockConnector Output { get; set; }
    public CodeBlockConnector Input { get; set; }

    public bool IsCategoryCompatible {
        get {
            return BlockCompatibleWithConnector(Output.BlockAttachedTo, Input) &&
                BlockCompatibleWithConnector(Input.BlockAttachedTo, Output);
        }
    }

    public float Distance {
        get => Vector3.Distance(Output.DistanceReferencePoint.position, Input.DistanceReferencePoint.position);
    }

    private bool BlockCompatibleWithConnector(CodeBlock block, CodeBlockConnector connector)
    {
        if (connector.CompatibleBlocks.Count == 0) return true;
        return connector.CompatibleBlocks.Contains(block.Category);
    }
}