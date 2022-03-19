using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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


    [Header("Blockly Connection")]

    [SerializeField] private string _blocklyTypeString;

    [SerializeField] private CodeBlockCategory _category;
    public CodeBlockCategory Category { get => this._category; }

    [SerializeField] private List<CodeBlockField> _blocklyFields;
    public List<CodeBlockField> BlocklyFields { get => this._blocklyFields; } 

    private string _id;

    private CodeBlockSize _size;

    private List<CodeBlockConnector> _outputConnectors = new List<CodeBlockConnector>();
    private List<CodeBlockConnector> _inputConnectors = new List<CodeBlockConnector>();
    private List<InputFinder> _inputFinders = new List<InputFinder>();

    private CodeBlockContainer _currentContainer;

    private CodeBlockInteractionManager _codeBlockInteractionManager;
    private CodeBlockConnectionManager _codeBlockConnectionManager;

    private XRSimpleInteractable _interactable;

    public string ID { get => this._id; }

    public string BlocklyTypeString { get => this._blocklyTypeString; }

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

    public bool IsRootBlock
    {
        get {
            bool atLeastOneConnectorHasPreviousFlow = false;
            foreach (var connector in this._connectors)
            {
                bool isPreviousConnector = connector.ConnectionFlow == CodeBlockConnector.Flows.Previous;
                if (isPreviousConnector)
                    atLeastOneConnectorHasPreviousFlow = true;
                if (connector.IsConnected && isPreviousConnector) 
                    return false;
            }
            return atLeastOneConnectorHasPreviousFlow;
        }
    }

    public List<CodeBlockConnector> AllConnectors { get => this._connectors; }

    public CodeBlockSize Size { get => this._size; }

    void Awake()
    {
        this._codeBlockInteractionManager = FindObjectOfType<CodeBlockInteractionManager>();
        this._codeBlockConnectionManager = FindObjectOfType<CodeBlockConnectionManager>();
        this._interactable = GetComponent<XRSimpleInteractable>();
        this._size = this.GetComponent<CodeBlockSize>();

        this._id = System.Guid.NewGuid().ToString("N");

        this.SetupConnectors();
        this.GetAllInputFinders();
    }

    void Start()
    {
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
        var interactor = this._interactable.firstInteractorSelecting as XRRayInteractor;
        bool shouldDetach = this.IsCurrentlyBeingMoved && !this.IsSolo;
        if (shouldDetach)
        {
            CodeBlockConnector detachmentPoint = FindBestDetachementPoint();
            this._codeBlockConnectionManager.DetachConnector(detachmentPoint);
            this._audioSource.PlayOneShot(this._detachSound, 1.0f);
        }

        this.MakeUserGrabSelfAndConnectedBlocks(interactor, playGrabSound: !shouldDetach);
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

    public void MakeUserGrabSelfAndConnectedBlocks(XRRayInteractor interactor, bool playGrabSound)
    {
        if (playGrabSound)
            this._audioSource.PlayOneShot(this._selectSound, 0.1f);

        RaycastHit raycastHit;
        var raycastHitExist = interactor.TryGetCurrent3DRaycastHit(out raycastHit);
        var spawnPosition = raycastHitExist ? (raycastHit.point + this.transform.forward * this.transform.localScale.z * 0.5f) : this.transform.position;

        var newContainer = this.CreateNewContainer(spawnPosition);
        var blocksToMoveToContainer = this.GetBlockCluster();
        foreach (var block in blocksToMoveToContainer)
        {
            block.MoveToContainer(newContainer);
        }
        this._codeBlockInteractionManager.MakeInteractorGrabContainer(newContainer, interactor, offsetGrab: true);
    }

    private CodeBlockContainer CreateNewContainer(Vector3 spawnPosition)
    {
        var newContainer = Instantiate(_containerPrefab, spawnPosition, this.transform.rotation);
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

    public IEnumerable<PotentialConnection> GetAllPotentialConnections(bool includeIncompatibleConnections = false)
    {
        foreach (var inputFinder in this._inputFinders)
        {
            foreach (var potentialConnection in inputFinder.PotentialConnections)
            {
                if (!includeIncompatibleConnections && !potentialConnection.IsCategoryCompatible) continue;
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

[Serializable]
public class CodeBlockField
{
    public string Name;
    public InputBase Input;
}