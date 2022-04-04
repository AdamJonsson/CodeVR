using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VariableDropdownHandler : MonoBehaviour
{
    [SerializeField] private DropdownInput _dropdown;
    public DropdownInput DropdownInput { get => this._dropdown; }

    private VariableDeclarationManager _variableDeclerationManager;

    private string _startValueByVariableID = null;
    private string StartValueByVariableID {
        get 
        {
            if (this._startValueByVariableName == null || this._startValueByVariableName == "") return this._startValueByVariableID;
            var foundVariable = this._variableDeclerationManager.Variables.Find(
                (variable) => variable.Name == this._startValueByVariableName
            );
            if (foundVariable == null) return null;
            return foundVariable.ID;
        }
    }
    
    [SerializeField] private string _startValueByVariableName = null;

    // Start is called before the first frame update
    void Start()
    {
        this._variableDeclerationManager = FindObjectOfType<VariableDeclarationManager>();
        this._variableDeclerationManager.OnVariablesChanged += this.OnVariablesChanged;
        this.ChangeDropdownValues(this.StartValueByVariableID);
    }

    private void OnVariablesChanged()
    {
        if (this == null) return;
        this.ChangeDropdownValues();
    }

    private void ChangeDropdownValues(string overrideStartValueWithVariableID = null)
    {
        var currentSelectedValue = overrideStartValueWithVariableID ?? this._dropdown.Value;
        var newDropdownValues = this._variableDeclerationManager.Variables.Select((variable) => {
            return new DropdownInput.DropdownOption() {
                Text = variable.Name,
                Value = variable.ID,
                IsDefaultValue = variable.ID == currentSelectedValue
            };
        }).ToList();

        this._dropdown.SetOptions(newDropdownValues);
    }

    public void SetStartValueByVariableID(string variableID)
    {
        this._startValueByVariableID = variableID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
