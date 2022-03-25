using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VariableNameForm : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _doneButton;
    [SerializeField] private TextInput _textInput;

    public Action<string> OnDone;
    public Action OnClose;

    // Start is called before the first frame update
    void Start()
    {
        this._doneButton.onClick.AddListener(this.OnDonePressed);
        this._closeButton.onClick.AddListener(this.OnClosePressed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInputValue(string inputValue)
    {
        this._textInput.SetValue(inputValue);
    }

    private void OnDonePressed()
    {
        if (this.OnDone != null) this.OnDone.Invoke(this._textInput.Value);
    }

    private void OnClosePressed()
    {
        if (this.OnClose != null) this.OnClose.Invoke();
    }
}
