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

    [Header("Specific settings")]
    [SerializeField] private Vector3 _rotation = Vector3.zero;
    [SerializeField] private Categories _connectionCategory = Categories.Row;
    [SerializeField] private Types _connectionType = Types.Input;
    [SerializeField] private CodeBlock _blockAttachedTo;

    [Header("Global settings")]
    [SerializeField] private float _connectionDistance = 0.25f;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private Transform _distanceReferencePoint;
    [SerializeField] private ParticleSystem _connectionParticleSystem;

    private InputFinder _inputFinder;

    private CodeBlockConnector _connection;

    public CodeBlock BlockAttachedTo { get => _blockAttachedTo; }
    public CodeBlockConnector Connection { get => _connection; }

    public CodeBlock BlockConnectedTo 
    { 
        get {
            if (_connection == null) return null;
            return _connection.BlockAttachedTo;
        }
    }

    public Transform DistanceReferencePoint { get => _distanceReferencePoint; }

    public bool IsConnected { get => _connection != null; }

    public Categories ConnectionCategory { get => _connectionCategory; }

    public Types ConnectionType { get => _connectionType; }

    public Pose ConnectionPose 
    {
        get {
            return new Pose(
                this.transform.position + this.transform.up * this.transform.lossyScale.x * (1.0f + _connectionDistance),
                this.BlockAttachedTo.transform.rotation
            );
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        this._inputFinder = GetComponent<InputFinder>();
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
        this._inputFinder.ClearPotentialConnections();
        if (snapConnectorsBlockCluster)
        {
            connector.BlockAttachedTo.SnapBlockClusterToConnector(this);
        }
    }

    public void Detach()
    {
        this._connection = null;
    }
}   
