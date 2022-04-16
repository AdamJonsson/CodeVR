using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(InputFinder))]
public class CodeBlockConnector : MonoBehaviour
{
    public enum Categories
    {
        Row,
        Column
    }

    public enum Types
    {
        Input,
        Output
    }

    public enum Flows
    {
        Next,
        Previous,
        None,
    }


    [Header("Specific settings")]

    [SerializeField] private Vector3 _rotation = Vector3.zero;

    [SerializeField] private Categories _connectionCategory = Categories.Row;
    public Categories ConnectionCategory { get => _connectionCategory; }

    [SerializeField] private Types _connectionType = Types.Input;
    public Types ConnectionType { get => _connectionType; }

    [SerializeField] private Flows _connectionFlow = Flows.None;
    public Flows ConnectionFlow { get => _connectionFlow; }

    [SerializeField] private CodeBlock _blockAttachedTo;
    public CodeBlock BlockAttachedTo { get => _blockAttachedTo; set => this._blockAttachedTo = value; }

    public CodeBlock BlockConnectedTo 
    { 
        get {
            if (_connection == null) return null;
            return _connection.BlockAttachedTo;
        }
    }

    [SerializeField] private List<CodeBlockCategory> _compatibleBlocks = new List<CodeBlockCategory>();
    public List<CodeBlockCategory> CompatibleBlocks { get => this._compatibleBlocks; }


    [Header("Blockly settings")]

    [SerializeField] private string _xmlTag;
    public string XmlTag { get => this._xmlTag; } 

    [SerializeField] private string _nameAttributeValue;
    public string NameAttributeValue { get => this._nameAttributeValue; }


    [Header("Global settings")]

    [SerializeField] private float _connectionDistance = 0.25f;
    public float ConnectionDistance { get => this._connectionDistance; }

    [SerializeField] private BoxCollider _collider;

    [SerializeField] private Transform _distanceReferencePoint;
    public Transform DistanceReferencePoint { get => _distanceReferencePoint; }

    [SerializeField] private ParticleSystem _connectionParticleSystem;

    private InputFinder _inputFinder;

    private CodeBlockConnector _connection;
    public CodeBlockConnector Connection { get => _connection; }
    public bool IsConnected { get => _connection != null; }

    public Action OnConnectionChanged;

    public Pose ConnectionPose 
    {
        get {
            return new Pose(
                this.transform.position + this.transform.up * this.transform.lossyScale.x * (1.0f + _connectionDistance),
                this.BlockAttachedTo.transform.rotation
            );
        }
    }

    /// <summary>Is always null if the connector is an input</summary>
    public InputFinder InputFinder { get => this._inputFinder; }

    void Awake()
    {
        this._inputFinder = GetComponent<InputFinder>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Application.isPlaying)
            this.NotifyMissingFields();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isPlaying) this.ApplyRequiredSetttings();
    }

    private void ApplyRequiredSetttings()
    {
        if (_collider != null)
            this._collider.isTrigger = ConnectionType == Types.Output;
        
        if (_inputFinder != null)
            this._inputFinder.enabled = ConnectionType == Types.Output;

        if (ConnectionType == Types.Output)
            this.gameObject.layer = LayerMask.NameToLayer("CodeBlockConnectorOutput");

        if (ConnectionType == Types.Input)
            this.gameObject.layer = LayerMask.NameToLayer("CodeBlockConnectorInput");

        if (ConnectionFlow == Flows.Next)
        {
            this._xmlTag = "next";
            this._nameAttributeValue = null;
        }
    }

    private void NotifyMissingFields()
    {
        bool isPrefabMode = this.gameObject.scene.rootCount == 0;
        if (isPrefabMode) return;

        if (this._blockAttachedTo == null) Debug.LogError("A connector do not have a reference to the block it is attached to. Check the fields for the connector");
    }

    public void Connect(CodeBlockConnector connector, bool snapConnectorsBlockCluster)
    {
        this._connection = connector;
        this._connectionParticleSystem.Play();
        if (this._inputFinder) this._inputFinder.ClearPotentialConnections();
        if (snapConnectorsBlockCluster)
        {
            connector.BlockAttachedTo.SnapBlockClusterToConnector(this);
        }
        
        if (this.OnConnectionChanged != null)
            this.OnConnectionChanged.Invoke();
    }

    public void Detach()
    {
        this._connection = null;
        if (this.OnConnectionChanged != null)
            this.OnConnectionChanged.Invoke();
    }

    public void UpdateBlocklyConnectionSetting(string xmlTag, string nameAttributeValue)
    {
        this._xmlTag = xmlTag;
        this._nameAttributeValue = nameAttributeValue;
    }
}   
