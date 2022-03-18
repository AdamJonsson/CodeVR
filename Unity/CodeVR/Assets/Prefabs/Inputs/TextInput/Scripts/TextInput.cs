using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInput : InputBase
{

    [SerializeField] private VRKeyboard _vrKeyboard;

    [SerializeField] private Button _button;
    public Button Button { get => this._button; }

    [SerializeField] private TMPro.TMP_Text _buttonText;

    public event Action<string> OnChange;

    private string _value = "";
    public override string Value { get => this._value; }

    private RectTransform _rectTransform;
    public RectTransform RectTransform { get => this._rectTransform; }

    // Start is called before the first frame update
    void Start()
    {
        this._button.onClick.AddListener(this.OnInputFocus);
        this._rectTransform = this._button.GetComponent<RectTransform>();

        this._vrKeyboard.OnChange += OnKeyboardInput;
        this._vrKeyboard.OnClose += OnCloseKeyboard;
    }

    private void OnInputFocus()
    {
        _vrKeyboard.gameObject.SetActive(true);
    }

    private void OnInputBlur()
    {
        _vrKeyboard.gameObject.SetActive(false);
    }

    private void OnCloseKeyboard()
    {
        _vrKeyboard.gameObject.SetActive(false);
    }

    private void OnKeyboardInput(string letter)
    {
        if (letter.ToUpper() == "SPACE") letter = " ";
        if (letter.ToUpper() == "BACK")
        {
            this._value = this._value.Remove(this._value.Length - 1);
        }
        else
        {
            this._value += letter;
        }

        this._buttonText.text = _value;
        this.OnChange.Invoke(this._value);
    }
}
