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
    }

    private void NotifyMissingFields()
    {
        bool isPrefabMode = this.gameObject.scene.rootCount == 0;
        if (isPrefabMode) return;

        if (this._blockAttachedTo == null) Debug.LogError("A connector do not have a reference to the block it is attached to. Check the fields for the connector");
    }
}   
