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

    [SerializeField] private string _startValue = "";

    [SerializeField] private string _valueIfEmpty = "Text";

    private string _value = "";
    public override string Value { get => this._value; }

    private RectTransform _rectTransform;

    public override RectTransform RectTransform { get => this._rectTransform; }

    private BlocklyCodeManager _blocklyCodeManager;

    // Start is called before the first frame update
    void Start()
    {
        this._button.onClick.AddListener(this.OnInputFocus);
        this._rectTransform = this._button.GetComponent<RectTransform>();

        this._vrKeyboard.OnChange += OnKeyboardInput;
        this._vrKeyboard.OnClose += OnCloseKeyboard;

        this._blocklyCodeManager = FindObjectOfType<BlocklyCodeManager>();

        this._value = this._startValue;

        this.UpdateButtonText(this._value);
        if (this.OnChange != null)
            this.OnChange.Invoke(this._value);
    }

    public void SetValue(string newValue)
    {
        this._value = newValue;
        this.UpdateButtonText(this._value);
        if (this.OnChange != null)
            this.OnChange.Invoke(this._value);
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
            if (this._value.Length == 0) return;
            this._value = this._value.Remove(this._value.Length - 1);
        }
        else
        {
            this._value += letter;
        }

        if (this._blocklyCodeManager != null)
            this._blocklyCodeManager.GenerateBlocklyCode();

        this.UpdateButtonText(this._value);
        if (this.OnChange != null)
            this.OnChange.Invoke(this._value);
    }

    private void UpdateButtonText(string text)
    {
        if (text == "")
        {
            this._buttonText.text = this._valueIfEmpty;
            this._buttonText.fontStyle = TMPro.FontStyles.Normal;
            return;
        }

        this._buttonText.text = text;
        this._buttonText.fontStyle = TMPro.FontStyles.Bold;
    }
}
