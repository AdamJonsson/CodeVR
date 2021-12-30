using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLine : MonoBehaviour
{
    [SerializeField] LineRenderer _line;

    [SerializeField] float _staticMargin = 0.02f;
    [SerializeField] float _expandSize = 0.02f;
    [SerializeField] float _expandSpeed = 1.0f;

    [SerializeField] AnimationCurve _expandAnimationCurve = AnimationCurve.Linear(0, 1, 1, 1);

    private float _expandAnimationCurveValue = 0.0f;

    private Vector3 _start = Vector3.zero;
    private Vector3 _end = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        _expandAnimationCurveValue += Time.deltaTime * _expandSpeed;
        if (_expandAnimationCurveValue > 1) _expandAnimationCurveValue = 0.0f;
        this.RenderLine();
    }

    private void RenderLine()
    {
        Vector3 currentMargin = (this._expandSize * this._expandAnimationCurve.Evaluate(this._expandAnimationCurveValue) + this._staticMargin) * this.CurrentLineDirection;
        _line.SetPositions(new Vector3[]{
            this._start - currentMargin,
            this._end + currentMargin,
        });
    }

    public void SetPositions(Vector3 start, Vector3 end)
    {
        this._start = start;
        this._end = end;
    }

    private Vector3 CurrentLineDirection { get => (this._start - this._end).normalized; }
}
