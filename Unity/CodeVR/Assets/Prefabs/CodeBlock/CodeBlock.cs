using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlock : MonoBehaviour
{
    [SerializeField] private List<CodeBlockConnector> _connectors;
    [SerializeField] private CodeBlockContainer _containerPrefab;
    [SerializeField] private AudioClip _selectSound;
    [SerializeField] private AudioClip _connectSound;
    [SerializeField] private AudioClip _detachSound;
    [SerializeField] private AudioSource _audioSource;

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

    void Start()
    {
        this.SetupConnectors();
        this._interactable = GetComponent<XRSimpleInteractable>();
        this._codeBlockInteractionManager = FindObjectOfType<CodeBlockInteractionManager>();
        this._codeBlockConnectionManager = FindObjectOfType<CodeBlockConnectionManager>();
        this.GetAllInputFinders();
        this._interactable.selectEntered.AddListener(OnUserSelected);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnection()
    {
        this._audioSource.PlayOneShot(_connectSound, 1.0f);
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
            this._audioSource.PlayOneShot(this._selectSound, 0.25f);
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
        this._codeBlockInteractionManager.MakeInteractorGrabContainer(newContainer, interactor);
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

        this.transform.SetPositionAndRotation(
            connectorToSnapTo.ConnectionPose.position,
            connectorToSnapTo.ConnectionPose.rotation
        );

        foreach (var connector in this._connectors)
        {
            if (!connector.IsConnected) continue;
            if (connectorToSnapTo.Connection == connector) continue;
            connector.BlockConnectedTo.SnapBlockClusterToConnector(connector);
        }
    }

    public bool BlockIsPartOfCluster(CodeBlock block)
    {
        var blockCluster = this.GetBlockCluster(true);
        return blockCluster.Contains(block);
    }
}
