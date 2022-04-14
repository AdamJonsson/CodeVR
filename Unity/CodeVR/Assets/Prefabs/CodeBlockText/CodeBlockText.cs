using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class CodeBlockText : MonoBehaviour
{
    [SerializeField] private string _startText = "ABC";
    [SerializeField] private List<TMP_Text> _textObjects;
    [SerializeField] private float _width = 1.0f;
    [SerializeField] private float _height = 1.0f;
    [SerializeField] private float _margin = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        SetText(_startText);
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateTextElements();
    }

    public void SetStartText(string text) => this._startText = text;

    private void UpdateTextElements()
    {
        if (Application.isPlaying) return;
        this.SetText(_startText);
        this.SetDimentions();
    }

    private void SetDimentions()
    {
        foreach (var textObject in this._textObjects)
        {
            textObject.rectTransform.sizeDelta = new Vector2(this._width - this._margin, this._height - this._margin);
        }
    }

    public void SetText(string text)
    {
        foreach (var textObject in this._textObjects)
        {
            textObject.text = text;
        }
    }
}
