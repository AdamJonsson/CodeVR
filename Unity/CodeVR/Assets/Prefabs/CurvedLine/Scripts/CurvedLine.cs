using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

[ExecuteInEditMode]
public class CurvedLine : MonoBehaviour
{
    [SerializeField] private Transform StartTransform;
    [SerializeField] private Transform EndTransform;
    [SerializeField] private float _curveAmount = 0.5f;
    [SerializeField] private LineRenderer _lineRenderer;

    // Update is called once per frame
    void Update()
    {
        if (this.StartTransform == null || this.EndTransform == null) return;

        this._lineRenderer.SetPositions( new Vector3[] {
            this.StartTransform.position,
            this.StartTransform.position + this.StartTransform.forward * _curveAmount,
            this.EndTransform.position - this.EndTransform.forward * _curveAmount,
            this.EndTransform.position
        });

    }

    private float DistanceBetweenStartAndEnd
    {
        get => Vector3.Distance(this.StartTransform.position, this.EndTransform.position);
    }
}
