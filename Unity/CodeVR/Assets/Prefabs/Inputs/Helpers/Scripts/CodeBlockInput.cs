using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeBlockInput : MonoBehaviour
{

    [SerializeField] private Button _button;
    [SerializeField] private TMP_Text _buttonText;

    [SerializeField] private RectTransform _canvas;

    public void SetText(string newText)
    {
        this._buttonText.text = newText;
    }

    public void SetCanvasSize(Vector2 size)
    {
        this._canvas.sizeDelta = size;
    }

    public RectTransform Canvas { get => this._canvas; }

    public Button Button { get => this._button; }

    public Vector2 CanvasSize { get => this._canvas.sizeDelta; }

    public Vector2 CanvasScale { get => this._canvas.localScale; }

    public void SetTransparency(float alpha)
    {
        var newColors = this._button.colors;
        newColors.normalColor = new Color(
            newColors.normalColor.r, 
            newColors.normalColor.g, 
            newColors.normalColor.b, 
            alpha
        );
        this._button.colors = newColors;
    }
}
