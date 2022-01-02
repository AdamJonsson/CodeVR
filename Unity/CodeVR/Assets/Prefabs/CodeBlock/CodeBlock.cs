using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CodeBlock : MonoBehaviour
{
    [SerializeField] private List<CodeBlockConnector> _connectors;
    [SerializeField] private CodeBlockContainer _containerPrefab;
    [SerializeField] private AudioClip _selectSound;
    [SerializeField] private AudioSource _audioSource;

    private CodeBlockConnector _outputConnector;
    private List<CodeBlockConnector> _inputConnectors;
    private InputFinder _inputFinder;

    private CodeBlockContainer _currentContainer;

    private CodeBlockInteractionManager _codeBlockInteractionManager;

    private XRSimpleInteractable _interactable;

    public bool HasContainer { get => _currentContainer != null; }

    public bool IsCurrentlyBeingMoved { get => this.HasContainer; }

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
        this._inputFinder = this._outputConnector.GetComponent<InputFinder>();
        this._audioSource = GetComponent<AudioSource>();

        this._interactable.selectEntered.AddListener(OnUserSelected);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnUserSelected(SelectEnterEventArgs args)
    {
        var interactor = this._interactable.firstInteractorSelecting;
        this.MakeUserGrabSelfAndConnectedBlocks(interactor);
        this._audioSource.PlayOneShot(this._selectSound);
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
        this._outputConnector = this._connectors.Find((connector) => connector.ConnectionType == CodeBlockConnector.Types.Output);
        this._inputConnectors = this._connectors.FindAll((connector) => connector.ConnectionType == CodeBlockConnector.Types.Input);
    }

    public List<PotentialConnection> GetAllPotentialConnections()
    {
        return this._inputFinder.PotentialConnections;
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
        }

        return allBlocks;
    }


}
