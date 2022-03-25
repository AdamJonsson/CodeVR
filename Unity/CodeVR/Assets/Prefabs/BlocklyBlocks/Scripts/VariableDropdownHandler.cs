using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VariableDropdownHandler : MonoBehaviour
{
    [SerializeField] private DropdownInput _dropdown;
    public DropdownInput DropdownInput { get => this._dropdown; }

    private VariableDeclarationManager _variableDeclerationManager;

    public string OverrideStartValue = null;

    // Start is called before the first frame update
    void Start()
    {
        this._variableDeclerationManager = FindObjectOfType<VariableDeclarationManager>();
        this._variableDeclerationManager.OnVariablesChanged += this.OnVariablesChanged;
        this.ChangeDropdownValues(this.OverrideStartValue);
    }

    private void OnVariablesChanged()
    {
        if (this == null) return;
        this.ChangeDropdownValues();
    }

    private void ChangeDropdownValues(string selectValue = null)
    {
        var currentSelectedValue = selectValue ?? this._dropdown.Value;
        var newDropdownValues = this._variableDeclerationManager.Variables.Select((variable) => {
            return new DropdownInput.DropdownOption() {
                Text = variable.Name,
                Value = variable.ID,
                IsDefaultValue = variable.ID == currentSelectedValue
            };
        }).ToList();

        this._dropdown.SetOptions(newDropdownValues);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
