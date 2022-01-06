using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ExpandableBlock : MonoBehaviour
{
    [Header("Scaling Values")]
    [SerializeField] private Vector3 _expandScale = Vector3.one;

    [Tooltip("Values in vector should vary between 1 and -1")]
    [SerializeField] private Vector3 _expandAnchor = Vector3.zero;

    [Header("Objects References")]
    [SerializeField] private List<ExpandableSetting> _expandables = new List<ExpandableSetting>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var expandable in this._expandables)
        {
            if (expandable.ShouldScale)
                expandable.TransformReference.localScale = this._expandScale;
            
            expandable.TransformReference.localPosition = expandable.Offset + Vector3.Scale(this._expandScale - Vector3.one, this._expandAnchor + expandable.ScaleOffset) / 2.0f;
        }
    }


    [Serializable]
    public struct ExpandableSetting
    {
        public Transform TransformReference;

        public Vector3 Offset;

        public Vector3 ScaleOffset;

        public bool ShouldScale;
    }
}
