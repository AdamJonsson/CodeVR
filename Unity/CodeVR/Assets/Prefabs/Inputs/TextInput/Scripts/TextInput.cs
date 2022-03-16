using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInput : InputBase
{

    [SerializeField] private TMPro.TMP_InputField _inputField;

    public override string Value { get => "A"; }


    // Start is called before the first frame update
    void Start()
    {
        this._inputField.Select();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
