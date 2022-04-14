using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterSection : MonoBehaviour
{
    [SerializeField] private ExpandableBlock _expandableBlock;
    public ExpandableBlock ExpandableBlock { get => this._expandableBlock; }

    [SerializeField] private CodeBlockConnector _connector;
    public CodeBlockConnector Connector { get => this._connector; }

    [SerializeField] private CodeBlockText _text;
    public CodeBlockText Text { get => this._text; }

    [SerializeField] private BoxCollider _collider;
    public BoxCollider Collider { get => this._collider; }

    public ExpandableBlock.ExpandableSetting ExpandableSetting 
    {
        get
        {
            return new ExpandableBlock.ExpandableSetting {
                TransformReference = this.gameObject.transform,
                Offset = new Vector3(0.0f, -1.0f, 0.0f),
                ScaleOffset = new Vector3(0.0f, -1.0f, 0.0f),
                ScaleFactor = Vector3.one,
                ShouldScale = false,
                ExtraStaticScale = Vector3.zero
            };
        }
    }
}
