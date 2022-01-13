using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CodeBlockSize))]
public class CodeBlock : MonoBehaviour
{
    [SerializeField] private List<CodeBlockConnector> _connectors;
    [SerializeField] private CodeBlockContainer _containerPrefab;

    [Header("Sound")]
    [SerializeField] private AudioClip _selectSound;
    [SerializeField] private AudioClip _connectSound;
    [SerializeField] private AudioClip _detachSound;
    [SerializeField] private AudioSource _audioSource;

    private CodeBlockSize _size;

    private List<CodeBlockConnector> _outputConnectors = new List<CodeBlockConnector>();
    private List<CodeBlockConnector> _inputConnectors = new List<CodeBlockConnector>();
    private List<InputFinder> _inputFinders = new List<InputFinder>();

    private CodeBlockContainer _currentContainer;

    private CodeBlockInteractionManager _codeBlockInteractionManager;
    private CodeBlockConnectionManager _codeBlockConnectionManager;

    private XRSimpleInteractable _interactable;

    public bool HasContainer { get => _currentContainer != null; }

    public CodeBlockContainer Container { get => _currentContainer; }

    public bool IsCurrentlyBeingMoved 
    { 
        get {
            if (!this.HasContainer) return false;
            return !this._currentContainer.HasDeleteFlag;
        } 
    }

    /// <summary>True if this block is not connected to any other block</summary>
    public bool IsSolo 
    { 
        get {
            foreach (var connector in this._connectors)
            {
                if (connector.IsConnected) return false;
            }
            return true;
        } 
    }

    public List<CodeBlockConnector> AllConnectors { get => this._connectors; }

    public CodeBlockSize Size { get => this._size; }

    void Start()
    {
        this.SetupConnectors();
        this._interactable = GetComponent<XRSimpleInteractable>();
        this._codeBlockInteractionManager = FindObjectOfType<CodeBlockInteractionManager>();
        this._codeBlockConnectionManager = FindObjectOfType<CodeBlockConnectionManager>();
        this._size = this.GetComponent<CodeBlockSize>();
        this.GetAllInputFinders();
        this._interactable.selectEntered.AddListener(OnUserSelected);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnectionToThisBlock()
    {
        this._audioSource.PlayOneShot(_connectSound, 1.0f);
        this.NotifyBlockClusterOfNewConnection();
    }

    private void OnNewConnectionToCluster()
    {
        this._size.ResizeAllExpandableBlocks();
        foreach (var connector in this._outputConnectors)
        {
            connector.InputFinder.ClearPotentialConnections();
        }
    }

    private void OnDetachConnectionFromCluster()
    {
        this._size.ResizeAllExpandableBlocks();
    }

    private void NotifyBlockClusterOfNewConnection()
    {
        var blocksToNotify = this.GetBlockCluster(true);
        foreach (var block in blocksToNotify)
        {
            block.OnNewConnectionToCluster();
        }
    }

    public void NotifyBlockClusterOfDetachement()
    {
        var blocksToNotify = this.GetBlockCluster(true);
        foreach (var block in blocksToNotify)
        {
            block.OnDetachConnectionFromCluster();
        }
    }

    private void GetAllInputFinders()
    {
        foreach (var outputConnector in this._outputConnectors)
        {
            this._inputFinders.Add(outputConnector.GetComponent<InputFinder>());
        }
    }

    public void OnUserSelected(SelectEnterEventArgs args)
    {
        var interactor = this._interactable.firstInteractorSelecting;
        if (this.IsCurrentlyBeingMoved && !this.IsSolo)
        {
            CodeBlockConnector detachmentPoint = FindBestDetachementPoint();
            this._codeBlockConnectionManager.DetachConnector(detachmentPoint);
            this._audioSource.PlayOneShot(this._detachSound, 1.0f);
        }
        else 
        {
            this._audioSource.PlayOneShot(this._selectSound, 0.1f);
        }

        this.MakeUserGrabSelfAndConnectedBlocks(interactor);
    }

    private CodeBlockConnector FindBestDetachementPoint()
    {
        foreach (var connector in this._connectors)
        {
            if (!connector.IsConnected) continue;
            var blocks = connector.BlockConnectedTo.GetBlockCluster(true, connector.Connection);
            if (blocks.Contains(this.Container.CodeBlockOrigin)) return connector;
        }
        return null;
    }

    private void MakeUserGrabSelfAndConnectedBlocks(IXRSelectInteractor interactor)
    {
        var newContainer = this.CreateNewContainer();
        var blocksToMoveToContainer = this.GetBlockCluster();
        foreach (var block in blocksToMoveToContainer)
        {
            block.MoveToContainer(newContainer);
        }
        this._codeBlockInteractionManager.MakeInteractorGrabContainer(newContainer, interactor, offsetGrab: true);
    }

    private CodeBlockContainer CreateNewContainer()
    {
        var newContainer = Instantiate(_containerPrefab, this.transform.position, this.transform.rotation);
        newContainer.SetCodeBlockOrigin(this);
        return newContainer;
    }

    public void MoveToContainer(CodeBlockContainer container)
    {
        this.transform.parent = container.transform;
        container.AddCodeBlock(this);
        this._currentContainer = container;
    }

    public void MoveOutFromContainer(CodeBlockContainer container)
    {
        if (this.transform.parent != container.transform) return;
        this.transform.parent = null;
        this._currentContainer = null;
    }

    private void SetupConnectors()
    {
        this._outputConnectors = this._connectors.FindAll((connector) => connector.ConnectionType == CodeBlockConnector.Types.Output);
        this._inputConnectors = this._connectors.FindAll((connector) => connector.ConnectionType == CodeBlockConnector.Types.Input);
    }

    public IEnumerable<PotentialConnection> GetAllPotentialConnections()
    {
        foreach (var inputFinder in this._inputFinders)
        {
            foreach (var potentialConnection in inputFinder.PotentialConnections)
            {
                yield return potentialConnection;
            }
        }
    }

    private void SetContainer(CodeBlockContainer container)
    {
        this._currentContainer = container;
    }

    public List<CodeBlock> GetBlockCluster(bool includeSelf = true, CodeBlockConnector connectorToIgnore = null)
    {
        var allBlocks = new List<CodeBlock>();
        if (includeSelf) allBlocks.Add(this);
        if (this.IsSolo) return allBlocks;

        foreach (CodeBlockConnector connector in this._connectors)
        {
            if (!connector.IsConnected) continue;
            if (connector == connectorToIgnore) continue;
            var blockFromConnection = connector.BlockConnectedTo.GetBlockCluster(true, connector.Connection);
            allBlocks.AddRange(blockFromConnection);
        }

        return allBlocks;
    }

    /// <summary>Snaps the blocks block-cluster to a connector</summary>
    public void SnapBlockClusterToConnector(CodeBlockConnector connectorToSnapTo)
    {
        if (!connectorToSnapTo.IsConnected) return;

        if (connectorToSnapTo.BlockAttachedTo.HasContainer)
        {
            this.MoveToContainer(connectorToSnapTo.BlockAttachedTo.Container);
        }
        else if (this.HasContainer)
        {
            this.MoveOutFromContainer(this._currentContainer);
        }

        this.AlignConnectors(connectorToSnapTo.Connection, connectorToSnapTo);

        foreach (var connector in this._connectors)
        {
            if (!connector.IsConnected) continue;
            if (connectorToSnapTo.Connection == connector) continue;
            connector.BlockConnectedTo.SnapBlockClusterToConnector(connector);
        }
    }

    /// <summary>Moves this block such that the two connectors align with each other</summary>
    public void AlignConnectors(CodeBlockConnector connectorAttachedToThisBlock, CodeBlockConnector otherConnectorToAlignWith)
    {
        this.transform.rotation = otherConnectorToAlignWith.ConnectionPose.rotation;

        // A pos offset is needed as the connector of the block is not always in the center.
        var posOffset = this.transform.position - connectorAttachedToThisBlock.transform.position;

        this.transform.position = otherConnectorToAlignWith.ConnectionPose.position + posOffset;
    }

    public void RealignBlockCluster(CodeBlockConnector connectorToIgnore = null)
    {
        foreach (var connector in AllConnectors)
        {
            if (!connector.IsConnected) continue;
            if (connector == connectorToIgnore) continue;
            connector.BlockConnectedTo.AlignConnectors(connector.Connection, connector);
        }
        foreach (var connector in AllConnectors)
        {
            if (!connector.IsConnected) continue;
            if (connector == connectorToIgnore) continue;
            connector.BlockConnectedTo.RealignBlockCluster(connector.Connection);
        }
    }

    public bool BlockIsPartOfCluster(CodeBlock block)
    {
        var blockCluster = this.GetBlockCluster(true);
        return blockCluster.Contains(block);
    }
}
