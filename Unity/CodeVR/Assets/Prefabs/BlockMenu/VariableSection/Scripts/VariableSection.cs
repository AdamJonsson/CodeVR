using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariableSection : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text _titleText;
    [SerializeField] private TMPro.TMP_Text _blockText;
    [SerializeField] private BlockDuplication _blockDuplication;

    private VariableDeclaration _variableDecleration;
    
    // Start is called before the first frame update
    void Start()
    {
        this._blockDuplication.OnClone += this.OnClone;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClone(CodeBlock block)
    {
        var dropdownHandler = block.GetComponent<VariableDropdownHandler>();
        dropdownHandler.OverrideStartValue = this._variableDecleration.ID;
    }

    public void SetVariableDecleration(VariableDeclaration variableDeclaration)
    {
        this._variableDecleration = variableDeclaration;
        this._titleText.text = variableDeclaration.Name;
        this._blockText.text = variableDeclaration.Name;
    }
}
